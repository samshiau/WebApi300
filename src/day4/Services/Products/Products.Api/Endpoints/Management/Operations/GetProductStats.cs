using Marten;
using Products.Api.Endpoints.Management.ReadModels;

namespace Products.Api.Endpoints.Management.Operations;

public static class GetProductStats
{
    public static async Task<IResult> GetProductInventoryChangeHistoryAsync(
        Guid id, IDocumentSession session)
    {
        var readModel = await session.LoadAsync<InventoryChangeReport>(id);
        if (readModel is not null)
        {
            await session.SaveChangesAsync();
            return TypedResults.Ok(readModel);
        }
        else
        {
            return TypedResults.NotFound();
        }
    }
}