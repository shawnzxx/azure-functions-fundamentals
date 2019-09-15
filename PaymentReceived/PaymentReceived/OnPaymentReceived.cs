using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentReceived.models;

namespace PaymentReceived
{
    public static class OnPaymentReceived
    {
        [FunctionName("OnPaymentReceived")]
        public static async Task<IActionResult> Run(
            //AuthorizationLevel.Function need to pass in secret code to function if you test cloud, local will ignore
            //We can overwrite the Route seetings, currently it uses default value: XXX/api/OnPaymentReceived
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            //Two output bindings
            //Apply Queue attribute to the function prameter, if not write Connection aurgument program will use default "AzureWebJobsStorage" inside local.settings.json
            //there is several parameter types we can bind to, IAsyncCollector<T> works well for Async method
            //IAsyncCollector interface allow us to easily to add row into the table storage
            [Queue("orders", Connection = "AzureWebJobsStorage")] IAsyncCollector<Order> orderQueue,
            [Table("orders", Connection = "AzureWebJobsStorage")] IAsyncCollector<OrderTable> orderTable,
            ILogger log)
        {
            log.LogInformation("Received a payment");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order order = JsonConvert.DeserializeObject<Order>(requestBody);

            await orderQueue.AddAsync(order);
            log.LogInformation($"Order {order.OrderId} successfully insert into the queue");

            OrderTable orderForTable = new OrderTable()
            {
                Email = order.Email,
                OrderId = order.OrderId,
                PartitionKey = "orders", //just one partition for demo purposes
                Price = order.Price,
                ProductId = order.ProductId,
                RowKey = order.OrderId
            };

            await orderTable.AddAsync(orderForTable);

            log.LogInformation($"Order {order.OrderId} successfully insert into the table");

            return new OkObjectResult($"Thank you for your purchase");
        }
        
    }
}
