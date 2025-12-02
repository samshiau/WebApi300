using Marten;
using Products.Api.Endpoints.Events;

namespace Products.Api.Endpoints.Operations;

public static class DeleteProduct
{

    public static async Task<IResult> DeleteByIdAsync(Guid id, IDocumentSession session)
    {
        // some things for tomorrow here.. like the version, but...
        session.Events.Append(id, new ProductDiscontinued());
        await session.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
