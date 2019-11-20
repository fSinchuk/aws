using System;
using System.Linq;
using System.IO;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;

namespace SQS.Reader
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(configuration["seecretAccess"], configuration["seecretKey"]);
            var sqsClient = new AmazonSQSClient(awsCredentials, RegionEndpoint.APSoutheast2);

            var queueUrl = sqsClient.GetQueueUrlAsync("EmailQueue").Result.QueueUrl;

            var reciveMessageRequest = new ReceiveMessageRequest {
                QueueUrl = queueUrl
            };

            var reciveMessageResponse = sqsClient.ReceiveMessageAsync(reciveMessageRequest).Result;
            foreach (var message in reciveMessageResponse.Messages) {
                Console.WriteLine("Message");
                
                Console.WriteLine($"MessageId {message.MessageId}");
                Console.WriteLine($"Body {message.Body}");
            }

            var messageReceptHandler = reciveMessageResponse.Messages.FirstOrDefault()?.ReceiptHandle;
            var deleteRequest = new DeleteMessageRequest {
                QueueUrl = queueUrl,
                ReceiptHandle = messageReceptHandler
            };

            var result = sqsClient.DeleteMessageAsync(deleteRequest).Result.HttpStatusCode;

            Console.WriteLine($"Message deleted {result}");

            Console.ReadKey();
        }
    }
}
