using JasperFx.Events;
using Products.Api.Endpoints.Management.Events;

namespace Products.Api.Endpoints.Management.ReadModels;

public record InventoryChangeReport
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public required string ProductName { get; set; }
    public List<InventoryRecord> Changes { get; set; } = [];

    public DateTimeOffset? DiscontinuedOn { get; set; } = null;

    public static InventoryChangeReport Create(IEvent<ProductCreated> @event)
    {
        var inventoryRecord = new InventoryRecord(@event.Timestamp, 0, @event.Data.Qty, "Initial");
        return new InventoryChangeReport { ProductName = @event.Data.Name, Changes = [inventoryRecord] };
    }

    public InventoryChangeReport Apply(IEvent<ProductQtyIncreased> @event, InventoryChangeReport model)
    {
        var lastChange = model.Changes.FirstOrDefault()?.After ?? 0;
        var newRecord = new InventoryRecord(@event.Timestamp, @event.Data.Increase, lastChange, "Increase");
        return model with { Changes = [newRecord, .. model.Changes] };
    }

    public InventoryChangeReport Apply(IEvent<ProductQtyDecreased> @event, InventoryChangeReport model)
    {
        var lastChange = model.Changes.FirstOrDefault()?.After ?? 0;
        var newRecord = new InventoryRecord(@event.Timestamp, -@event.Data.Decrease, lastChange, "Decrease");
        return model with { Changes = [newRecord, .. model.Changes] };
    }

    public InventoryChangeReport Apply(IEvent<ProductDiscontinued> @event, InventoryChangeReport model)
    {
        return model with { DiscontinuedOn = @event.Timestamp };
    }
}

public record InventoryRecord(DateTimeOffset When, int Before, int After, string Kind)
{
    public int Change => After - Before;
}