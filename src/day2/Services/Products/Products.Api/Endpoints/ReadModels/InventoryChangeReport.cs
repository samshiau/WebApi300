using JasperFx.Events;
using Products.Api.Endpoints.Events;

namespace Products.Api.Endpoints.ReadModels;

public record InventoryChangeReport
{
    public Guid  Id { get; set; }
    public int Version { get; set; }
    public List<InventoryRecord> Changes { get; set; } = [];

    public static InventoryChangeReport Create(ProductCreated @event)
    {
        return new InventoryChangeReport();
    }

    public InventoryChangeReport Apply(IEvent<ProductQtyInventoryAdjusted> @event, InventoryChangeReport model)
    {
        var lastChange = model.Changes.FirstOrDefault();
        decimal oldValue = 0;
        decimal delta = 0;
        if(lastChange is not null)
        {
            oldValue = lastChange.OldValue;
            delta = @event.Data.NewQty - oldValue;
        }
        var newRecord = new InventoryRecord(@event.Timestamp, @event.Data.NewQty, oldValue, delta);

        return model with { Changes = [newRecord, .. model.Changes] };
    }


}

public record InventoryRecord(
    DateTimeOffset When, 
   Decimal OldValue,
   Decimal Delta,
    Decimal Change);
