using Facet;
using ImTools;
using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Products.Api.Endpoints.Handlers;
using Wolverine;

namespace Products.Api.Endpoints.Operations;

// mapping is going from a -> b
// get a postcreaterequest -> command -> postcreateresponse



// ProductCreateRequest has to be used to create the CreateProduct command.
[Facet(typeof(CreateProduct), exclude: ["Id"])]
public partial record ProductCreateRequest;



[Facet(typeof(CreateProduct))]
public partial record ProductCreateResponse
{ 
    public string Status => "Pending";
}

[Facet(typeof(AdjustProductInventory))]
public partial record AdjustProductInventoryRequest;

[Facet(typeof(AdjustProductInventory))]
public partial record AdjustProductInventoryResponse;


public static class PostProduct
{
    public static async Task<Ok<ProductCreateResponse>> AddProductToInventoryAsync(
        ProductCreateRequest request,
        IMessageBus messaging
        )
    {
        var command = new CreateProduct(Guid.NewGuid(), request.Name, request.Price, request.Qty);
        await messaging.PublishAsync( command );
        return TypedResults.Ok(new ProductCreateResponse(command));
    }

    // POST /products/{id}/inventory-adjustments
    public static async Task<IResult> AdjustProductInventory(
        Guid id, 
        AdjustProductInventoryRequest request,
        IMessageBus messageBus
        )
    {

        // get the current version from the database, or whatever, if it doesn't match,
        // return an error (Http Conflict usually)
        // I have a product with a qty of 200. It is version 88.
        // I get a post to here that says change the qty to 12, but they send me the version 
        // they are working from (85, or whatever)
        // Compare the two. You can't change that.
        // Who can do this? (Authn/authz)
        // can't do to something that doesn't exist, etc.
        // check to see if we have a product with that id, if not, return a 404.
        // Also, the version thing... but I'll come back to that.
        var command = new AdjustProductInventory(request.Id, request.Version, request.newQty);
        await messageBus.PublishAsync(command);
        return TypedResults.Accepted($"/products/{id}");
    }
}
