using System.Security.Claims;
using JasperFx.Events;
using Marten;
using Products.Api.Endpoints.Management.Events;
using Products.Api.Endpoints.Management.ReadModels;

namespace Products.Api.Endpoints.Management.Handlers;

public class ProductsHandler
{
    public async Task<UserInfo> LoadAsync(IDocumentSession session, IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var sub = user?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? throw new UnauthorizedAccessException();
        var info = await session.Query<UserInfo>().Where(u => u.Sub == sub).SingleOrDefaultAsync();

        if (info is not null) return info;

        var userId = Guid.NewGuid(); 
        session.Events.StartStream(userId, new ProductUserCreated(userId, sub));
        await session.SaveChangesAsync();
        return new UserInfo
        {
            Id = userId,
            Sub = sub
        };
    }


    // POST endpoint is the source of this command
    public StreamAction Handle(CreateProduct command, IDocumentSession session, UserInfo userInfo)
    {
        var (id, name, price, qty) = command;
        return session.Events.StartStream(id, new ProductCreated(id, name, price, qty, userInfo.Id));
    }

    public void Handle(DiscontinueProduct command, IDocumentSession session)
    {
        session.Events.Append(command.Id, new ProductDiscontinued(command.Id));
    }

    public void Handle(IncreaseProductQty command, IDocumentSession session)
    {
        session.Events.Append(command.Id, new ProductQtyIncreased(command.Id, command.Increase));
    }

    public void Handle(DecreaseProductQty command, IDocumentSession session)
    {
        session.Events.Append(command.Id, new ProductQtyDecreased(command.Id, command.Decrease));
    }
}