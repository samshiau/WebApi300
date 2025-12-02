using Microsoft.OpenApi;
using ShoppingServiceDefaults.OpenApiTransforms;

namespace Products.Api.Infra;

public class ServiceOpenApiTransform : ShoppingCoreOAuth2DocumentTransformer
{
    public override IDictionary<string, string> NeededScopes { get; set; } = new Dictionary<string, string>
    {
        
       // TODO: Define needed scopes here
        // { "openid", "Access the OpenID Connect user profile" }
        
    };

    public override OpenApiInfo Info { get; set; } = new()
    {
        // TODO: Define OpenAPI Info here
        Title = "Products API",
        Version = "v1",
        Description = "API for Product Management and Customer Browsing"

    };
}


