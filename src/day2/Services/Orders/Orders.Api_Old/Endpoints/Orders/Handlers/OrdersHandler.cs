using Orders.Api.Endpoints.Orders.Operation;

namespace Orders.Api.Endpoints.Orders.Handlers;


// Note: With Wolverine, you EITHER have to write configuration code (in Program.cs) to associate commands
// with handlers, or you can use (or even create your own) conventions.
// The fact that this class ends in the word "Handler" is important.
public class OrdersHandler(ILogger<OrdersHandler> logger)
{

    // You can write code to configure this, but the name "Handle" means something here. It will "wire it up".

    public async Task HandleAsync(ProcessOrder command)
    {
        //if(command.Cart.Amount % 2 != 0)
        //{
        //    throw new Exception("Can only process even numbers");
        //}
        await Task.Delay(2000);
        logger.LogInformation("Got an order! For {show}, amount: {amt:c}", 
            command.Cart.CustomerName, command.Cart.Amount);
    }
}
