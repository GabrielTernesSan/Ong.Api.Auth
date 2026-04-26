using MediatR;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using Ong.Application;
using Ong.Application.Requests;
using Ong.Infra;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraLayer(builder.Configuration);
builder.Services.AddApplicationLayer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddMassTransit(x => x.UsingInMemory());

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddRuntimeInstrumentation();
        metrics.AddPrometheusExporter();
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ong Auth API", Version = "v1" });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();

app.UseAuthentication();

app.MapHealthChecks("/health");
app.UseOpenTelemetryPrometheusScrapingEndpoint("/metrics");

app.MapPost("/auth/register", async ([FromBody] RegisterRequest request, IMediator mediator) =>
{
    var result = await mediator.Send(request);
    return result.HasErrors ? Results.BadRequest(result) : Results.Ok(result);
}).WithTags("Auth");

app.MapPost("/auth/login", async ([FromBody] LoginRequest request, IMediator mediator) =>
{
    var result = await mediator.Send(request);
    return result.HasErrors ? Results.Unauthorized() : Results.Ok(result);
}).WithTags("Auth");

app.Run();
