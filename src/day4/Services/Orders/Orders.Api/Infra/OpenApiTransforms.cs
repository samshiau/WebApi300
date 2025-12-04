using Microsoft.OpenApi;
using ShoppingServiceDefaults.OpenApiTransforms;

namespace Orders.Api.Infra;

public class ServiceOpenApiTransform : ShoppingCoreOAuth2DocumentTransformer
{
    public override IDictionary<string, string> NeededScopes { get; set; } = new Dictionary<string, string>
    {
        // TODO: Define needed scopes here
        // { "openid", "Access the OpenID Connect user profile" }
    };

    public override OpenApiInfo Info { get; set; } = new()
    {
        Title = "Order Processing API",
        Version = "v1",
        Description = "API for processing customer orders."
    };
}