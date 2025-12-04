using Orders.Api.Endpoints.Orders.Operation;
using Orders.Api.Endpoints.Orders.Services;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Orders.Api.Endpoints.Orders;

public static class OrderApi
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOrders()
        {
            services.AddScoped<CardProcessor>();
            return services;
        }
    }

    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapOrders()
        {
            // I know this will all be stuff for orders, right? so METHOD /orders/ 
            var ordersGroup = builder.MapGroup("/orders");

            ordersGroup.MapGet("/", () => "Your Orders");

            ordersGroup.MapPost("/", Post.AddOrderAsync);

            return builder;
        }
    }
}

public record ShoppingCartRequest : IValidatableObject
{
    [Required] [Range(1, 3000)] public decimal Amount { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var customerName = CustomerName.ToLowerInvariant().Trim();
        if (customerName == "jeff gonzalez" && Amount > 10M)
            yield return new ValidationResult("Jeff Is a mooch and can't buy that much");
        if (customerName.Contains("vader") && Amount > 500)
            yield return new ValidationResult("Sith Lords Not To Be Trusted");
    }
}

public enum OrderStatus
{
    Received,
    Processing,
    Complete,
    Failed
}; // etc. etc.

public record OrderResponse
{
    public Guid Id { get; set; }

    public OrderStatus Status { get; set; }
}