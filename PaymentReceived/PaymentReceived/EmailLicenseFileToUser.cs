using System;
using System.IO;
using System.Text.RegularExpressions;
using PaymentReceived.models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace PaymentReceived
{
    public static class EmailLicenseFileToUser
    {
        [FunctionName("EmailLicenseFileToUser")]
        public static void Run(
            //previous step we saved as string so here we use string input binding
            [BlobTrigger("licenses/{orderId}.lic", Connection = "AzureWebJobsStorage")]string licenseFileContents,
            [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> sender,
            //This is input binding, 3 inputs are TableName, PartitionKey and RowKey
            [Table("orders", "orders", "{orderId}")] Order order,
            //get blob file name as an orderId
            string orderId, 
            ILogger log)
        {
            //var email = Regex.Match(licenseFileContents,
            //    @"^Email\:\ (.+)$", RegexOptions.Multiline).Groups[1].Value;

            var email = order.Email;
            log.LogInformation($"Got order from {email}\n Order Id:{orderId}");

            var message = new SendGridMessage();
            //this will get Environment Variable from local.settings.json
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
            message.AddTo(email);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(licenseFileContents);
            var base64 = Convert.ToBase64String(plainTextBytes);
            message.AddAttachment($"{orderId}.lic", base64, "text/plain");
            message.Subject = "Your license file";
            message.HtmlContent = "Thank you for your order";

            //only send an email when is not end with @test.com
            if (!email.EndsWith("@test.com")) {
                sender.Add(message);
            }

            log.LogInformation($"email send to the user");
        }
    }
}
