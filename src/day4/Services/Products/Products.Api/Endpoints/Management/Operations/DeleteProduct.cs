using Marten;
using Products.Api.Endpoints.Management.Events;
using Products.Api.Endpoints.Management.Handlers;
using Wolverine;

namespace Products.Api.Endpoints.Management.Operations;

public static class DeleteProduct
{
    public static async Task<IResult> DiscontinueProductAsync(Guid id, IMessageBus bus)
    {
        await bus.PublishAsync(new DiscontinueProduct(id));
        return TypedResults.NoContent();
    }
}