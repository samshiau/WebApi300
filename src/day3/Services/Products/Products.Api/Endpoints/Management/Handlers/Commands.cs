namespace Products.Api.Endpoints.Management.Handlers;

public record CreateProduct(Guid Id, string Name, decimal Price, int Qty);

public record DiscontinueProduct(Guid Id);

public record IncreaseProductQty(Guid Id, int Increase);

public record DecreaseProductQty(Guid Id, int Decrease);