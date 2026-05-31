using API.Extensions;
using BuildingBlocks.Identity;
using BuildingBlocks.Monitoring;
using BuildingBlocks.Logging.Extensions;
using Abdulhadi.Foundation.Identity.Application;
using Abdulhadi.Foundation.Identity.Api.Extensions;
using Abdulhadi.Foundation.Identity.Infrastructure;
using Abdulhadi.Foundation.Identity.Infrastructure.Persistence;
using Abdulhadi.Foundation.Identity.Infrastructure.Authentication.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedLogging();
builder.Services.AddMonitoring(builder.Configuration);

builder.Services.AddCorsPolicy();
builder.Services.AddControllers();
builder.Services.AddRateLimiting();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorizationPolicies();
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddExternalAuthentication(builder.Configuration);

// Layers
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

// Swagger
builder.Services.AddSwaggerDocumentation();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UsePresentation();

app.Run();