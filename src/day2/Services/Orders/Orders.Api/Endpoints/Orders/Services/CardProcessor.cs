namespace Orders.Api.Endpoints.Orders.Services
{
    public class CardProcessor(ILogger<CardProcessor> logger)

    {

        public async Task<string> ProcessCardAsync(string customerName, CancellationToken token)
        {
            // write the code to do this later
            logger.LogInformation("Processing a card for {customerName}", customerName);
            await Task.Delay(1000);
            if (customerName == "Jeff Gonzalez")
            {
                logger.LogError("Processing Blew Up for {customer}", customerName);
                throw new Exception("Blammo!");
            }
            return "33333999"; // confirmation number
        }
    }
}
