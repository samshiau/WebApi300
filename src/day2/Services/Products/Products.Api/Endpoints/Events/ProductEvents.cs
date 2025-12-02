namespace Products.Api.Endpoints.Events;

// Things that have happened. So we use past-tense in the naming.
// And these are usually the things the "business" cares about.




public record ProductCreated(Guid Id, string Name, decimal Price, int Qty);

public record ProductPriceAdjusted(decimal NewPrice);

public record ProductQtyInventoryAdjusted(int NewQty);

public record ProductDiscontinued();