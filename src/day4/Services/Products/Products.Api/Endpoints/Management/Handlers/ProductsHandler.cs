using System.Security.Claims;
using JasperFx.Events;
using Marten;
using Products.Api.Endpoints.Management.Events;
using Products.Api.Endpoints.Services;
using Wolverine;

namespace Products.Api.Endpoints.Management.Handlers;

public record SendProductToOrders(Guid Id);

// Sort of the same - but this is to indicate that that product is no longer available.
public record SendTombstoneProductToOrders(Guid Id);
public class ProductsHandler
{
    // whenever the criteria of a thing WE own changes in a way Orders needs, do this.


    // before
    // POST endpoint is the source of this command
    public async Task<StreamAction> Handle(CreateProduct command, IDocumentSession session,
        IProvideUserInfo userProvider)
    {
        var (id, name, price, qty, bySub) = command;
        var creator = await userProvider.GetUserInfoFromSubAsync(bySub);
        var @event = new ProductCreated(id, name, price, qty, creator!.Id);
        return session.Events.StartStream(id, @event);
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


    // After magic.
    // Ok, I met with the orders folks,
    // after we do our thing (using events and all that), here's what they seem to care about.
    public ValueTask After(IMessageContext context) =>
        context.Envelope?.Message switch
        {
            CreateProduct pc => context.PublishAsync(new SendProductToOrders(pc.Id)),
            DecreaseProductQty pqd => context.PublishAsync(new SendProductToOrders(pqd.Id)),
            IncreaseProductQty pqi => context.PublishAsync(new SendProductToOrders(pqi.Id)),
            DiscontinueProduct pd => context.PublishAsync(new SendTombstoneProductToOrders(pd.Id)),
            _ => ValueTask.CompletedTask
        };
}