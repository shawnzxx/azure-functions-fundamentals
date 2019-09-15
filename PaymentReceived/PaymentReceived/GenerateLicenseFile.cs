using System;
using System.IO;
using System.Threading.Tasks;
using PaymentReceived.models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace PaymentReceived
{
    public static class GenerateLicenseFile
    {
        /* 
         * Code refactoring: we use {rand-guid}.lic as generated file name, how to change to order.id as file name?
         * Answer is to use IBinder define attribute inside the function
        [FunctionName("GenerateLicenseFile")]
        public static void Run(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")]Order order,
            [Blob("licenses/{rand-guid}.lic")] TextWriter outputBlob,
            ILogger log)
        {
            outputBlob.WriteLine($"OrderId: {order.OrderId}");
            outputBlob.WriteLine($"Email: {order.Email}");
            outputBlob.WriteLine($"ProductId: {order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate: {DateTime.UtcNow}");
            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");

            log.LogInformation($"LicenseFile generated successfully");
        }
        */

        [FunctionName("GenerateLicenseFile")]
        public static async Task Run(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")]Order order,
            IBinder binder,
            ILogger log)
        {
            //IBinder interface need to provide two type of infos: 1: binding attribute 2: the type we binding to
            //For 1st info works with all binding attributes (eg. QueueAttibute, SendGridAttribute)
            //since we constructure the attribute inside the method body, we can calculate parameters on-demand in the body
            //not only we can customzie the path paramater we can also pass in different storage connection
            //for 2nd info Ibinder makes it very flexible to choose the type to bind to at runtime, here we choose TextWriter
            var outputBlob = await binder.BindAsync<TextWriter>(
                    new BlobAttribute($"licenses/{order.OrderId}.lic")
                    {
                        Connection = "AzureWebJobsStorage"
                    });

            outputBlob.WriteLine($"OrderId: {order.OrderId}");
            outputBlob.WriteLine($"Email: {order.Email}");
            outputBlob.WriteLine($"ProductId: {order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate: {DateTime.UtcNow}");
            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");

            log.LogInformation($"LicenseFile generated successfully");
        }
    }
}
