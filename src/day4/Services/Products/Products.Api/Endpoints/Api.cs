using Products.Api.Infra;
using Products.Api.Endpoints.Management.Operations;
using Products.Api.Endpoints.Services;

namespace Products.Api.Endpoints;

public static class ApiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddProducts()
        {
            services.AddScoped<IProvideUserInfo, UserInfoProvider>();
            return services;
        }
    }

    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapProductRoutes()
        {
            var group = builder.MapGroup("/products")
                .WithDisplayName("Product Management Operations").RequireAuthorization();

            group.MapGet("/{id:guid}/inventory-change-history",
                GetProductStats.GetProductInventoryChangeHistoryAsync);

            group.MapDelete("/{id:guid}", DeleteProduct.DiscontinueProductAsync);

            group.MapPost("/", PostProduct.AddProductAsync)
                .WithName("Add Product")
                .WithDisplayName("Add A Product To Inventory")
                .WithDescription("Add a product to the inventory store.");

            group.MapPost("/{id:guid}/inventory-increases", InventoryAdjustments.IncreaseQty);
            group.MapPost("/{id:guid}/inventory-decreases", InventoryAdjustments.DecreaseQty);

            group.MapGet("/{id:guid}", GetProduct.ByIdAsync);
            group.MapGet("/", GetProductsSummary.GetAllAsync);

            builder.MapGet("/manager", GetManagerSummary.GetCurrentManagerSummary)
                .RequireAuthorization();

            builder.MapGet("/managers/{id:guid}", GetManagerSummary.GetCurrentManagerSummary)
                .RequireAuthorization();
            return builder;
        }
    }
}