using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace ShoppingServiceDefaults.OpenApiTransforms;

/// <summary>
/// Disclaimer: This is classroom code. Not super production ready - I use something *close* to this in production,
/// but check with your local authorities. - Jeff
/// </summary>
public abstract class ShoppingCoreOAuth2DocumentTransformer : IOpenApiDocumentTransformer
{
    public abstract IDictionary<string, string> NeededScopes { get; set; }
    public abstract OpenApiInfo Info { get; set; }

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = Info;
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        var config = context.ApplicationServices.GetRequiredService<IConfiguration>();
        var uri = Assembly.IsBuildingOpenApiDocs()
            ? "http://localhost:9069"
            : config.GetValue<string>("Authentication:Schemes:OpenIdConnect:Authority") ??
              throw new Exception("Missing configuration OpenIdConnect:Authority");

        // Add OAuth2 security scheme (Authorization Code flow only)
        document.Components.SecuritySchemes.Add("oauth2",
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                OpenIdConnectUrl = new Uri(uri + "/.well-known/openid-configuration"),
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(uri + "/authorize"), TokenUrl = new Uri(uri + "/token")
                    }
                }
            });

        // Apply security requirement globally
        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("oauth2"), [..NeededScopes.Keys.ToArray()] }
            }
        ];

        // Set the host document for all elements
        // including the security scheme references
        document.SetReferenceHostDocument();
        return Task.CompletedTask;
    }
}