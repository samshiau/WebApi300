using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Products.Api.Endpoints.ReadModels;

namespace Products.Api.Endpoints.Operations;

public static  class GetProduct
{

    public static async Task<IResult> GetProductInventoryChangeHistoryAsync(
        Guid id, IDocumentSession session)
    {

        // this is the live - go through all the events RIGHT NOW.
        //  var readModel = await session.Events.AggregateStreamAsync<InventoryChangeReport>(id);

        var readModel = await session.LoadAsync<InventoryChangeReport>(id);
        // going to be maybe a bit behind.
        if (readModel is null)
        {
            return TypedResults.NotFound();
        }
        else
        {
            //session.Events.Append(id, new ProductRetrieved());
            await session.SaveChangesAsync();
            return TypedResults.Ok(readModel);
        }
       
    }

     // GET /products - returns a list of all the streams that are products, and projects them
     // into a SINGLE result that contains an array of stuff.
     // [ { id, name, price } ]
    public static async Task<IResult> GetProductByIdAsync(Guid id, IDocumentSession session)
    {

        // Go through EVERY event related to the stream with this id, and apply them to this read model

        // "Live" projection - it is, every time it runs, looking at all the data for this project,
        // throwing away any events that aren't part of my read model, and giving me the final version.
        // that read model is created dynamically on every request.

        // You can "pre aggregate" these - in other words, create a "table" in the database
        // that already had this in it.

        // When this happens - "Eventually" - Eventual Consistency.
        // It uses an "Async Daemon" (background worker) that will update the table in the database.
        // This can be spread across more nodes (instances) to do the work. 

        // for really complex projections 

        // We can do projections that are "transactionally consistent"
        // The read model will be updated on each event.
        // slows down writes, but means that you ALWAYS will have the most up to date data.




        // query the database and load the ProductDetails for this id.

        //var response = await session.LoadAsync<ProductDetails>(id); 
        var readModel = await session.Events.AggregateStreamAsync<ProductDetails>(id);

        if(readModel is null)
        {
            return TypedResults.NotFound();
        } else
        {
            //session.Events.Append(id, new ProductRetrieved());
            await session.SaveChangesAsync();
            return TypedResults.Ok(readModel);
        }

    }
}

//public record ProductRetrieved(string byWho);