using Marten.Events.Aggregation;
using Products.Api.Endpoints.Management.Events;

namespace Products.Api.Endpoints.Management.ReadModels;

public class UserInfoProjection : SingleStreamProjection<UserInfo, Guid>
{
    public static UserInfo Create(ProductUserCreated @event)
    {
        return new UserInfo
        {
            Id = @event.Id,
            Sub = @event.Sub
        };
    }
}