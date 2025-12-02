using Microsoft.AspNetCore.Http.HttpResults;
using Orders.Api.Endpoints.Orders.Services;
using Wolverine;

namespace Orders.Api.Endpoints.Orders.Operation;

public record ProcessOrder(Guid Id, ShoppingCartRequest Cart);
public static class Post
{
    public static async Task<Ok<OrderResponse>> AddOrderAsync(
        ShoppingCartRequest request,
        IMessageBus messageBus, // provided by Wolverine.. can be "wrapped" if you wanted.
        CancellationToken token)
    {

      
     
        var orderId = Guid.NewGuid();

        //await messageBus.InvokeAsync(command) - This is synchronous. Find the handler, and don't do anything until it returns. Good if it is going to return something.
        await messageBus.PublishAsync(new ProcessOrder(orderId, request));
        // awaiting that this has been published, not that it has been handled.

        



        var order = new OrderResponse
        {
            Id = orderId,
            Status = OrderStatus.Received
        };
        return TypedResults.Ok(order); // the caller only gets this.
    }
}

