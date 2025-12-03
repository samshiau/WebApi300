//using Microsoft.AspNetCore.Http.HttpResults;
//using Orders.Api.Endpoints.Orders.Services;

//namespace Orders.Api.Endpoints.Orders.Operation;

//public static class Post1
//{
//    public static async Task<Ok<Order>> AddOrderAsync(ShoppingCartRequest request, CardProcessor cardProccessor, CancellationToken token)
//    {
//        // That transaction list.
//        // validate the request

//        // do the early bound stuff we can do, then
//        // sechedule the rest for later.

//        // come back to this..
//        // arrange shipping
//        var shippingTask = Task.Delay(1000); // come back to this list when this is done
//                                             // charge card

//        // async tasks are not "fire and forget" - they are "fire and hope nothing bad happens"
//        var cardTask = cardProccessor.ProcessCardAsync(request.CustomerName, token);
//        // etc. 
//        // save it...
//        // then.
//        // WhenAll or WaitAll (both have their place, more later) waits until they are done,
//        // and will catch any exceptions.
//        await Task.WhenAll(shippingTask, cardTask);

//        var order = new Order
//        {
//            Id = Guid.NewGuid(),
//            Total = request.Amount * 1.13M
//        };
//        return TypedResults.Ok(order); // the caller only gets this.
//    }
//}


///* ordersGroup.MapPost("/",async (ShoppingCartRequest request, CardProcessor cardProccessor, CancellationToken token) =>
//            {


//            });*/