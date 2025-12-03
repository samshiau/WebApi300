using Orders.Api.Endpoints.Orders;

namespace Orders.Api.Endpoints;

public static class ApiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEndpoints()
        {
            services.AddOrders();
            return services;
        }
    }

    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpointRoutes()
        {
            builder.MapOrders();
            return builder;
        }
    }
}