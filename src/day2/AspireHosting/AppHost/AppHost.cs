using AppHost;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

#region Ambient Services (Services that are developer environment stuff)
// this is a classroom concession to show stuff.
var username = builder.AddParameter("username", "user");
var password = builder.AddParameter("password", "password");
var postgres = builder.AddPostgres("postgres",  username, password, port: 5432)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImage("postgres:17.5"); // You can use "custom" images too.


// in "production" will be your "real" identity server.
var identity = builder.AddMockOidcDevelopmentServer();

#endregion

#region DevTools

var scalarApis = builder.AddScalarApiReference("scalar-apis", 9561, options =>
    {
        options.DisableDefaultProxy();
        options.PreferHttpsEndpoint();
        options.PersistentAuthentication = true;
        options.AllowSelfSignedCertificates();
        options.AddPreferredSecuritySchemes("oauth2")
            .AddAuthorizationCodeFlow("oauth2",
                flow =>
                {
                    flow.WithClientId("aspire-client")
                        .WithClientSecret("super-secret")
                        .WithSelectedScopes("openid", "profile", "email", "roles");
                });

        options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
    })
    .WaitFor(identity);
#endregion

#region Services 

#region Orders.Api

var ordersDb = postgres.AddDatabase("orders");

var ordersApi = builder.AddProject<Projects.Orders_Api>("ordersapi")
    .WithReference(ordersDb)
    .WithEnvironment("identity", () => identity.GetEndpoint("http").Url)
    .WithIdentityOpenIdAuthority(identity)
    .WithIdentityOpenIdBearer(identity)
    .WaitFor(ordersDb)
    .WaitFor(identity);
scalarApis.WithApiReference(ordersApi, options => { options.AddDocument("orders.v1", "Order Processing API"); });
#endregion

#region Products.Api
var productsDb = postgres.AddDatabase("products"); // going to use my own database for this.

var productsApi = builder.AddProject<Projects.Products_Api>("productsapi")
    .WithReference(productsDb)
    .WithEnvironment("identity", () => identity.GetEndpoint("http").Url)
    .WithIdentityOpenIdAuthority(identity)
    .WithIdentityOpenIdBearer(identity)
    .WaitFor(productsDb)
    .WaitFor(identity);

scalarApis.WithApiReference(productsApi, options => { options.AddDocument("products.v1", "Product Management API"); });

#endregion
#endregion

builder.Build().Run();
