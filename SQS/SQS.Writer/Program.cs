using Microsoft.Extensions.Configuration;
using System;
using System.IO;

using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace SQS.Writer
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine(new string('*', 100));
            Console.WriteLine("SQS Service");
            Console.WriteLine(new string('*', 100));

            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(configuration["seecretAccess"], configuration["seecretKey"]);
            IAmazonSQS sqsClient = new AmazonSQSClient(awsCredentials, RegionEndpoint.APSoutheast2);
            Console.WriteLine("Create a queue called EmailQueue");

            var sqsRequest = new CreateQueueRequest { QueueName = "EmailQueue" };
            var createQueueReponse = sqsClient.CreateQueueAsync(sqsRequest).Result;

            var sqsUrl = createQueueReponse.QueueUrl;

            var listQueueRequest = new ListQueuesRequest();
            var listQueueResponse = sqsClient.ListQueuesAsync(listQueueRequest);

            foreach (var item in listQueueResponse.Result.QueueUrls)
            {
                Console.WriteLine($"Queue url: {item}");
            }

            Console.WriteLine("Send a message to Queue");

            var sqsMessageRequest = new SendMessageRequest
            {
                QueueUrl = sqsUrl,
                MessageBody = "First message"
            };

            sqsClient.SendMessageAsync(sqsMessageRequest);

            Console.WriteLine("Message sent");
            Console.ReadKey();
        }
    }
}
