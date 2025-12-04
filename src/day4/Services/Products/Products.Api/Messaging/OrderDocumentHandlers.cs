using Marten;
using Products.Api.Endpoints.Management.Events;
using Products.Api.Endpoints.Management.Handlers;
using Products.Api.Endpoints.Management.ReadModels;
using Wolverine;
using Wolverine.Attributes;

namespace Products.Api.Messaging;

// ok, I'm going to handle those SendToOrders thing, but what I want is to send a "document" to them.
public record OrdersApiProductDocument(ProductDetails? OrderProductDocument);

[WolverineHandler]
public static class OrderDocumentHandlers
{
    public static async ValueTask Handle(SendProductToOrders command, IDocumentSession session,
        IMessageBus bus, ILogger logger)
    {
        // might do a custom view model - look that up in the database, pick out the stuff you want to send to
        // the orders api, 
        var doc = await session.Events.FetchLatest<ProductDetails>(command.Id);
        if (doc is not null)
            await bus.PublishAsync(new OrdersApiProductDocument(doc), new DeliveryOptions
            {
                PartitionKey = command.Id.ToString(),
               
            });
    }

    public static async ValueTask Handle(SendTombstoneProductToOrders command, IMessageBus bus)
    {
        await bus.PublishAsync(new OrdersApiProductDocument(null), new DeliveryOptions
        {
            PartitionKey = command.Id.ToString()
        });
    }
}

//[WolverineHandler]
//public class DemoDocumentHandlers
//{
//    public static void Handle(OrdersApiProductDocument message, ILogger logger, Envelope env)
//    {
//        if (message.OrderProductDocument is null)
//            logger.LogWarning("Received tombstone for product document {Key}", env.PartitionKey);
//        else
//            logger.LogWarning("Received product document: {Product} Key: {key}", message.OrderProductDocument,
//                env.PartitionKey);
//    }
//}