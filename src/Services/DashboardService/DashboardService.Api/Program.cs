using DashboardService.Application.Interfaces;
using DashboardService.Application.UseCases.Dashboard.GetRecentVotes;
using DashboardService.Application.UseCases.Dashboard.GetSummary;
using DashboardService.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("VotesDb");
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetDashboardSummaryQuery).Assembly));


var jwtSettings = builder.Configuration.GetSection("Jwt");

var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);


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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("ADMIN"));
});


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


var dashboardGroup = app.MapGroup("/api/dashboard");


dashboardGroup.MapGet("/summary", async (IMediator mediator) =>
{
    var query = new GetDashboardSummaryQuery();
    var result = await mediator.Send(query);
    return Results.Ok(result);
})
.RequireAuthorization("RequireAdminRole");

dashboardGroup.MapGet("/recent-votes", async (IMediator mediator) =>
{
    var query = new GetRecentVotesQuery();
    var result = await mediator.Send(query);
    return Results.Ok(result);
})
.RequireAuthorization("RequireAdminRole");


app.MapGet("/", () => Results.Ok("Dashboard Service is running."));

app.Run();