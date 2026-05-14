using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Ong.Api.Auth.Extensions;
using Ong.Application;
using Ong.Application.Requests;
using Ong.Domain.Queries;
using Ong.Infra;
using OpenTelemetry.Metrics;
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

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Configuração Jwt:Key não encontrada.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Configuração Jwt:Issuer não encontrada.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Configuração Jwt:Audience não encontrada.");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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

app.MapGet("/auth/outbox", async ([FromServices] IOutboxMessageQuery query) =>
{
    var messages = await query.ObterOutboxMessagesPendentesAsync();
    return messages;
}).RequireApiKey();

app.MapPatch("/auth/outbox/{id}/processed", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new UpdateOutboxRequest() { Id = id });
    return result.HasErrors ? Results.BadRequest(result) : Results.Ok(result);
}).RequireApiKey();

app.MapPatch("/auth/outbox/{id}/error", async (Guid id, [FromBody] UpdateOutboxRequest request, IMediator mediator) =>
{
    request.Id = id;
    var result = await mediator.Send(request);
    return result.HasErrors ? Results.BadRequest(result) : Results.Ok(result);
}).RequireApiKey();

app.Run();
