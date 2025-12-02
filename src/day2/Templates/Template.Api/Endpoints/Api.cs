

namespace Template.Api.Endpoints;

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
        public IEndpointRouteBuilder MapRoutes()
        {
        

            return builder;
        }
    }
}
