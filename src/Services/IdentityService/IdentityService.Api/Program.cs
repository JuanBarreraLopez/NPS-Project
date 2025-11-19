using FluentValidation;
using IdentityService.Api;
using IdentityService.Application.Interfaces;
using IdentityService.Application.UseCases.Auth.Login;
using IdentityService.Application.UseCases.Auth.Refresh;
using IdentityService.Application.UseCases.Users.Create;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Text;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(builder.Configuration);
var connectionString = builder.Configuration.GetConnectionString("IdentityDb");

builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasherService>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(LoginCommandValidator).Assembly);

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

app.MapGroup("/api/identity").MapAuthApi();

app.MapGroup("/users").MapUserApi();

app.MapGet("/", () => Results.Ok("Identity Service is running."));

app.SeedDatabase();

app.Run();


public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthApi(this RouteGroupBuilder group)
    {
        group.MapPost("/login", async (LoginCommand command, IMediator mediator) =>
        {
            try
            {
                var validator = new LoginCommandValidator();
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                var result = await mediator.Send(command);

                return Results.Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Results.Json(new { message = ex.Message }, statusCode: StatusCodes.Status401Unauthorized);
            }
            catch (Exception)
            {
                return Results.Problem("Ocurrió un error inesperado.", statusCode: 500);
            }
        });

        group.MapPost("/refresh", async (RefreshTokenCommand command, IMediator mediator) =>
        {
            try
            {
                var validator = new RefreshTokenCommandValidator();
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                var result = await mediator.Send(command);

                return Results.Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Results.Json(new { message = ex.Message }, statusCode: StatusCodes.Status401Unauthorized);
            }
            catch (Exception)
            {
                return Results.Problem("Ocurrió un error inesperado.", statusCode: 500);
            }
        });

        return group;
    }




}

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group)
    {
        // Endpoint: POST /users/create
        group.MapPost("/create", async (CreateUserCommand command, IMediator mediator) =>
        {
            try
            {
                // 1. Validar la entrada
                var validator = new CreateUserCommandValidator();
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                // 2. Enviar el comando al handler
                var newUserId = await mediator.Send(command);

                // 3. Devolver el ID del nuevo usuario
                return Results.Created($"/users/{newUserId}", new { id = newUserId });
            }
            catch (ValidationException ex)
            {
                // Si el usuario ya existe
                return Results.Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return Results.Problem("Ocurrió un error inesperado.", statusCode: 500);
            }
        });

        return group;
    }
}