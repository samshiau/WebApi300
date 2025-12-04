using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Products.Api.Endpoints.Management.ReadModels;

namespace Products.Api.Endpoints.Management.Operations;

public static class GetProduct
{
    public static async Task<Results<Ok<ProductDetails>, NotFound>> ByIdAsync(
        Guid id,
        IDocumentSession session,
        IHttpContextAccessor context
    )
    {
        var readModel = await session.Events.FetchLatest<ProductDetails>(id);
        return readModel switch
        {
            null => TypedResults.NotFound(),
            _ => TypedResults.Ok(readModel)
        };
    }
}