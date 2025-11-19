using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NpsService.Application.Interfaces;
using NpsService.Application.UseCases.Votes.Submit;
using NpsService.Infrastructure.Repositories;
using NpsService.Infrastructure.Services;
using System.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("VotesDb");
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(SubmitVoteCommand).Assembly));


builder.Services.AddValidatorsFromAssembly(typeof(SubmitVoteCommandValidator).Assembly);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role,
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();


app.MapGroup("/api/nps").MapVoteApi();

app.MapGet("/", () => Results.Ok("Nps Service is running."));
app.Run();


public static class VoteEndpoints
{
    public static RouteGroupBuilder MapVoteApi(this RouteGroupBuilder group)
    {
        group.MapPost("/submit", async (SubmitVoteCommand command, IMediator mediator) =>
        {
            try
            {
                var validator = new SubmitVoteCommandValidator();
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                await mediator.Send(command);

                return Results.Ok(new { message = "Voto registrado exitosamente." });
            }
            catch (ValidationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return Results.Json(new { message = ex.Message }, statusCode: StatusCodes.Status401Unauthorized);
            }
            catch (Exception)
            {
                return Results.Problem("Ocurrió un error inesperado.", statusCode: 500);
            }
        })
        .RequireAuthorization();

        return group;
    }
}