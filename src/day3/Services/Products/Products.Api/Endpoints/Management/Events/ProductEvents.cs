using Facet;
using Products.Api.Endpoints.Management.Handlers;

namespace Products.Api.Endpoints.Management.Events;

public record ProductCreated(Guid Id, string Name, decimal Price, int Qty, Guid CreatedBy);

public record ProductUserCreated(Guid Id, string Sub);

public record ProductPriceAdjusted(decimal NewPrice);

[Facet(typeof(IncreaseProductQty))]
public partial record ProductQtyIncreased;

[Facet(typeof(DecreaseProductQty))]
public partial record ProductQtyDecreased;

public record ProductDiscontinued(Guid Id);