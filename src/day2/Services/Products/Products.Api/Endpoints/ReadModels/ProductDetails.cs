using JasperFx.Events;
using Products.Api.Endpoints.Events;

namespace Products.Api.Endpoints.ReadModels;


// So, what I want to do is go through all the events for a particular Product, and 
// boil it down to this..

public record ProductDetails
{
    public Guid Id { get; set;  }
    public int Version { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Qty { get; set; }

    public bool IsLowInventory => Qty < 10;
    public DateTimeOffset WhenAdded { get; set; }

    public DateTimeOffset? DateOfLastInventoryAdjustment { get; set; } = null;
    // Create here means that this even represents the birth of this stream, and should result
    // in a new read model. 
    public static ProductDetails Create(IEvent<ProductCreated> @event)
    {
        return new ProductDetails
        {
            Id = @event.Id,
            Name = @event.Data.Name,
            Price = @event.Data.Price,
            Qty = @event.Data.Qty,
            WhenAdded = @event.Timestamp
        };
    }

    // If any events of this type have happened, use this to "apply" that event to this read model.
    public ProductDetails Apply(IEvent<ProductQtyInventoryAdjusted> @event, ProductDetails oldVersion)
    {
        return oldVersion with { Qty = @event.Data.NewQty, DateOfLastInventoryAdjustment = @event.Timestamp };


    }

    public void Delete(ProductDiscontinued @event)
    {
        // this should no longer be stored.
    }
}
