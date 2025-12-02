using Template.Api.Endpoints;
using Template.Api.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddCorsForDevelopment();
builder.AddDevelopmentOpenApiGeneration("open-api-doc", "v1");

builder.Services.AddAuthenticationSchemes();
builder.Services.AddAuthorizationAndPolicies();

builder.AddPersistenceAndMessaging("database-name");

var app = builder.Build();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiForDevelopment();

// TODO: Map Endpoints Here

app.MapDefaultEndpoints();
app.Run();