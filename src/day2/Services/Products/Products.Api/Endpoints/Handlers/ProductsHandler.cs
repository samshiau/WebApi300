using Marten;
using Products.Api.Endpoints.Events;

namespace Products.Api.Endpoints.Handlers;

// commands "belong with" the handler for that command.
// a command can come from 1+ sources, but is always handled by one bit of code. The handler.

public record CreateProduct(Guid Id, string Name, decimal Price, int Qty);
public record AdjustProductInventory(Guid Id, int Version, int newQty); // talk about this, too.

public class ProductsHandler
{
    // Create a Product - This could mean many things.. but right now it means there is a new 
    // stream we want to have access to that is the life history of this product.
    public async Task HandleAsync(CreateProduct command, IDocumentSession session)
    {
        var (id, name,price, qty ) = command;
        // starting a stream of events all about the life of this product.
        // When a "thingy" (a stream) is brand new, you use StartStream.

        session.Events.StartStream(id, new ProductCreated(id, name, price, qty) );

        // public record productCreated
        // public record nameset(name)
        // public record priceSet(price)
        // public record qtyset(qty)
        
     //   session.Events.Append(command.CreatedBy, ...);
        
        await session.SaveChangesAsync();
    }

    public async Task HandleAsync(AdjustProductInventory command, IDocumentSession session)
    {
        // You COULD do more validation here - 
        session.Events.Append(command.Id, new ProductQtyInventoryAdjusted(command.newQty));
        await session.SaveChangesAsync();
    }
}