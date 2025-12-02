using Products.Api.Endpoints;
using Products.Api.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddCorsForDevelopment();
builder.AddDevelopmentOpenApiGeneration("products", "v1");

builder.Services.AddAuthenticationSchemes();
builder.Services.AddAuthorizationAndPolicies();

builder.AddPersistenceAndMessaging("products");

var app = builder.Build();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiForDevelopment();

app.MapProductRoutes();

app.MapDefaultEndpoints();
app.Run();