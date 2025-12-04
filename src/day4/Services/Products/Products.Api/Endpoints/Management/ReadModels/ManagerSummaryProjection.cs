using JasperFx.Events;
using Marten.Events.Projections;
using Products.Api.Endpoints.Management.Events;

namespace Products.Api.Endpoints.Management.ReadModels;

// This is mostly about A "ProductUser", but their ID will be "mentioned" in other events on other streams.

public class ManagerSummaryProjection : MultiStreamProjection<ManagerSummary, Guid>
{
    public ManagerSummaryProjection()
    {
        Identity<UserCreated>(e => e.Id);
        Identity<ProductCreated>(p => p.CreatedBy);
    }

    public static ManagerSummary Create(UserCreated @event)
    {
        return new ManagerSummary
        {
            Id = @event.Id,
            Sub = @event.Sub
        };
    }

    public void Apply(IEvent<ProductCreated> @event, ManagerSummary model)
    {
        var item = new ManagerProductItem(@event.Data.Name, @event.Timestamp);
        var link = $"/products/{@event.Id}";
        model.ProductsCreated.Add(link, item);
        KeyValuePair<string, ManagerProductItem> kvp = new(link, item);
    }
}