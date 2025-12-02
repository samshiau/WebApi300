using Marten;
using Orders.Api.Endpoints.Orders;
using Orders.Api.Endpoints.Orders.Operation;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wolverine;
using Wolverine.Marten;
using Wolverine.Postgresql;


var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults(); // this comes from the OrdersServicedDefaults


builder.AddNpgsqlDataSource("orders"); // This is going to look for the "orders" connection string. 
// you create one of these per database. They can be configured for resiliency, all that.

builder.Host.UseWolverine(options =>
{
    options.Policies.UseDurableLocalQueues(); // local is local, just stuff we are working on.
    options.Policies.UseDurableInboxOnAllListeners(); // this can list for commands from other services, make them durable, too
}); // add a few services, one in particular is IMessageBus.

builder.Services.AddMarten(options =>
    {
        options.DisableNpgsqlLogging = true;
    })
    .IntegrateWithWolverine()
    .UseLightweightSessions()
    .UseNpgsqlDataSource();

if(builder.Environment.IsDevelopment())
{
    // Wolverine specific, obviously, but it can be part of a cluster of "nodes"
    // sharing work. This tells it it is a loner, el solo lobo. Don't go looking for friends.
    builder.Services.RunWolverineInSoloMode();
}


builder.Services.AddOrders();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddValidation(); // turn on validation for all my API endpoints.

var app = builder.Build(); // one world above the build, one after.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapOrders(); // This will add all the operations for the "/orders" resource.
app.MapDefaultEndpoints(); // again, from ShoppingServiceDefaults

app.Run();
