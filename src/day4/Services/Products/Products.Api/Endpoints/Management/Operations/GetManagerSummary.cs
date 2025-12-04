using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Products.Api.Endpoints.Management.ReadModels;
using Products.Api.Endpoints.Services;

namespace Products.Api.Endpoints.Management.Operations;

// Whoever added a vendor, I want them to see a list of just the vendors they added.
// Maybe other things later - like what products did they add, etc.
public static class GetManagerSummary
{
    // todo: Pick up here tomorrow, show how to do the "Load" thing without Wolverine.
    public static async Task<Results<Ok<ManagerSummary>, NotFound>> GetCurrentManagerSummary(
        IProvideUserInfo userInfoProvider,
        IDocumentSession session)
    {
        var currentUser = await userInfoProvider.GetUserInfoAsync();
        var id = currentUser.Id;
        var summary = await session.LoadAsync<ManagerSummary>(id);
        return summary switch
        {
            null => TypedResults.NotFound(),
            _ => TypedResults.Ok(summary)
        };
    }

    public static async Task<Results<Ok<ManagerSummary>, NotFound>> GetSummaryForManager(
        Guid id,
        IDocumentSession session)
    {
        var summary = await session.LoadAsync<ManagerSummary>(id);
        return summary switch
        {
            null => TypedResults.NotFound(),
            _ => TypedResults.Ok(summary)
        };
    }
}