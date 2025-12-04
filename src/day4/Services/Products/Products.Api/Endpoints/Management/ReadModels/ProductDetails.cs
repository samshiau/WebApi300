using JasperFx.Events;
using Marten.Events.Aggregation;
using Products.Api.Endpoints.Management.Events;

namespace Products.Api.Endpoints.Management.ReadModels;

public record ProductDetails
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Qty { get; set; }

    public bool IsLowInventory => Qty < 10;
    public DateTimeOffset WhenAdded { get; set; }
}

public class ProductReadModelProjection : SingleStreamProjection<ProductDetails, Guid>
{
    public ProductReadModelProjection()
    {
        DeleteEvent<ProductDiscontinued>();
    }

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

    public ProductDetails Apply(ProductQtyIncreased @event, ProductDetails model)
    {
        return model with { Qty = model.Qty + @event.Increase };
    }

    public ProductDetails Apply(ProductQtyDecreased @event, ProductDetails model)
    {
        return model with { Qty = model.Qty + @event.Decrease };
    }
}