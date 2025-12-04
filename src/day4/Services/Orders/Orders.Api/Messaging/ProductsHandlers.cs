using Marten;
using Products.Api.Endpoints.Management.ReadModels;
using Products.Api.Messaging;
using Wolverine;
using Wolverine.Attributes;

namespace Orders.Api.Messaging;


/* {
		"id": "6c89b851-7aef-4310-98c9-bcb0390345bf",
		"version": 1,
		"name": "Pizza",
		"price": 12.99,
		"qty": 13,
		"isLowInventory": false,
		"whenAdded": "2025-12-04T20:50:02.2704789+00:00"
	}*/


public record Product
{
	public Guid Id { get; set; }
	public decimal Price { get; set; }
	public int Qty { get; set; }
}

[WolverineHandler]
public class ProductsHandlers(IDocumentSession session)
{

	public async Task Handle(OrdersApiProductDocument command, Envelope env, IMessageBus bus)
	{
	
		if(command.OrderProductDocument is null)
		{
            if (env.PartitionKey is null)
            {
				await bus.PublishAsync(new CouldNotProcessProduct());
				return;
            }
            var id = Guid.Parse(env.PartitionKey);
			
			session.DeleteWhere<ProductDetails>(d => d.Id == id);
		} else
		{
			session.Store<ProductDetails>(command.OrderProductDocument);

		}
		await session.SaveChangesAsync();
	}
}

public record CouldNotProcessProduct();
// Products.Api.Messaging.OrdersApiProductDocument