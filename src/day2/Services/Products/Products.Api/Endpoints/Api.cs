

using Products.Api.Endpoints.Operations;

namespace Products.Api.Endpoints;
public static class ApiExtensions
{

    extension(IServiceCollection services)
    {
        public IServiceCollection AddOrders()
        {
         
            return services;
        }
    }
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapProductRoutes()
        {

            var group = builder.MapGroup("/products");
            group.MapGet("/{id:guid}/inventory-change-history",
                GetProduct.GetProductInventoryChangeHistoryAsync);
            // .RequireAuthorization("Managers");

            group.MapDelete("/{id:guid}", DeleteProduct.DeleteByIdAsync);
            group.MapPost("/", PostProduct.AddProductToInventoryAsync);
            group.MapPost("/{id:guid}/inventory-adjustments", PostProduct.AdjustProductInventory);
            group.MapGet("/{id:guid}", GetProduct.GetProductByIdAsync);
            return builder;
        }
    }
}
