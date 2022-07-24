using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace ServiceBus
{
    class Program
    {
        // the client that owns the connection and can be used to create senders and receivers
        // Create the client object that will be used to create sender and receiver objects
        private static ServiceBusClient ServiceBusClient => QueueHandler.GetClient(Configuration.ServiceBusConnectionString);

        // the processor that reads and processes messages from the queue
        private static ServiceBusProcessor processor;

        // the sender used to publish messages to the queue
        static ServiceBusSender Sender => QueueHandler.GetSender(ServiceBusClient, Configuration.QueueName);

        // number of messages to be sent to the queue
        private const int numOfMessages = 3;

        public static async Task Main(string[] args)
        {
            // create a processor that we can use to process the messages
            processor = ServiceBusClient.CreateProcessor(Configuration.QueueName, new ServiceBusProcessorOptions());

            try
            {
                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();


                await SendMessageBtachAsync();

                Console.WriteLine("Press any key to end the application");
                Console.ReadKey();

                // stop processing
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");

            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception:{exception.Message}");

                throw;
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await processor.DisposeAsync();
                await ServiceBusClient.DisposeAsync();
            }
        }

        // handle received messages
        static async Task MessageHandler(ProcessMessageEventArgs args)
        
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }

        // handle any errors when receiving messages
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private static async Task SendMessageBtachAsync()
        {
            // create a batch 
            ServiceBusMessageBatch messageBatch = await Sender.CreateMessageBatchAsync();

            for (int i = 1; i <= 3; i++)
            {
                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i} sent to queue: {Configuration.QueueName} ")))
                {
                    // if an exception occurs
                    throw new Exception($"Exception {i} has occurred.");
                }
            }
            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await Sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                if (Sender != null)
                {
                    await Sender.DisposeAsync();
                }
            }
        }
    }
}