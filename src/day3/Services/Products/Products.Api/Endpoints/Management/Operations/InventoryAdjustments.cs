using System.ComponentModel.DataAnnotations;
using Marten;
using Products.Api.Endpoints.Management.Handlers;
using Products.Api.Endpoints.Management.ReadModels;
using Wolverine;

namespace Products.Api.Endpoints.Management.Operations;

public record QtyIncreaseRequest(
    Guid Id,
    [property: Required]
    [property: Range(1, int.MaxValue)]
    int Increase,
    int Version);

public record QtyDecreaseRequest(
    Guid Id,
    [property: Required]
    [property: Range(1, int.MaxValue)]
    int Decrease,
    int Version);

public static class InventoryAdjustments
{
    public static async Task<IResult> IncreaseQty(
        Guid id,
        QtyIncreaseRequest request,
        IMessageBus messageBus,
        IDocumentSession session
    )
    {
        var existing =
            await session.Events.FetchForWriting<ProductDetails>(id, request.Version);
        switch (existing.Aggregate)
        {
            case null:
                return TypedResults.Conflict();
            default:
            {
                var command = new IncreaseProductQty(id, request.Increase);
                await messageBus.InvokeAsync(command);
                return TypedResults.Accepted($"/products/{id}");
            }
        }
    }

    public static async Task<IResult> DecreaseQty(
        Guid id,
        QtyDecreaseRequest request,
        IMessageBus messageBus,
        IDocumentSession session
    )
    {
        var existing =
            await session.Events.FetchForWriting<ProductDetails>(id, request.Version);
        switch (existing.Aggregate)
        {
            case null:
                return TypedResults.Conflict();
            default:
            {
                var command = new DecreaseProductQty(id, request.Decrease);
                await messageBus.InvokeAsync(command);
                return TypedResults.Accepted($"/products/{id}");
            }
        }
    }
}