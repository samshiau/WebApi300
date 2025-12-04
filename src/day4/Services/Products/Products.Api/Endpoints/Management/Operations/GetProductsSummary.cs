using System.Security.Claims;
using Marten;
using Products.Api.Endpoints.Management.ReadModels;

namespace Products.Api.Endpoints.Management.Operations;

public static class GetProductsSummary
{
    public static async Task<IResult> GetAllAsync(IDocumentSession session, ClaimsPrincipal user)
    {
        var sub = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var readModels = await session.Query<ProductDetails>().ToListAsync();
        return Results.Ok(readModels);
    }
}