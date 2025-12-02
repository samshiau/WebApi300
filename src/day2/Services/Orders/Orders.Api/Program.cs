using Orders.Api.Endpoints;
using Orders.Api.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddCorsForDevelopment();
builder.AddDevelopmentOpenApiGeneration("orders", "v1");

builder.Services.AddAuthenticationSchemes();
builder.Services.AddAuthorizationAndPolicies();

builder.AddPersistenceAndMessaging("orders");

var app = builder.Build();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiForDevelopment();

app.MapEndpointRoutes();

app.MapDefaultEndpoints();
app.Run();