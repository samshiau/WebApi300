using Marten.Events.Projections;
using Products.Api.Endpoints.Management.Events;

namespace Products.Api.Endpoints.Management.ReadModels;


// This is mostly about A "ProductUser", but their ID will be "mentioned" in other events on other streams.

public class ManagerSummaryProjection : MultiStreamProjection<ManagerSummary, Guid>
{
    public ManagerSummaryProjection()
    {
        Identity<ProductUserCreated>(e => e.Id); 
        Identity<ProductCreated>(p => p.CreatedBy);
     
    }

    public static ManagerSummary Create(ProductCreated @event)
    {
        return new ManagerSummary
        {
            Id = @event.Id
        };
    }

    public ManagerSummary Apply(ProductCreated @event, ManagerSummary summary)
    {
        return summary with { CreatedProductIds = [..summary.CreatedProductIds, @event.Id] };
    }
}