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
    public static class Functions
    {
		
        /// <summary>
        /// Simply function triggered by HTTP and send a mesage to queue storage
        /// </summary>
        /// <param name="req"></param>
        /// <param name="msg"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("TestTrigger")]
        public static async Task<IActionResult> Run(
            //AuthorizationLevel.Anonymous every one can call this function on Azure without provide secret
            //Route = null mean follow "domain/api/HttpTrigger" http trigger pattern
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Queue("outqueue", Connection = "MyStorageAccountConnection")] ICollector<string> msg,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            if (!string.IsNullOrEmpty(name)) {
                // Add a message to the output collection.
                msg.Add(string.Format("Name passed to the function: {0}", name));
            }

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
