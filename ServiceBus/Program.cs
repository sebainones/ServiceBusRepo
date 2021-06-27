using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace ServiceBus
{
    class Program
    {

        //Send messages to an Azure Service Bus topic and receive messages from its subscriptions (.NET)
        //https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-how-to-use-topics-subscriptions

        //To send messages to a Topic
        private static ITopicClient topicClient;
        private static ITopicClient TopicClient => topicClient ??= TopicHandler.CreatTopicClient(Configuration.ServiceBusConnectionString, Configuration.TopicName);


        //To receive messages from a Subcription to a Topic
        //To receive messages, you must create a SubscriptionClient object, NOT a TopicClient object
        static ISubscriptionClient subscriptionClient;
        static ISubscriptionClient SubscriptionClient => subscriptionClient ??= new SubscriptionClient(Configuration.ServiceBusConnectionString, Configuration.TopicName, Configuration.SubscriptionName);


        //However, you'll use the TopicClient class instead of the QueueClient class to send messages and the SubscriptionClient class to receive messages.

        public static async Task Main(string[] args)
        {
            try
            {
                SubscribeToReeiveTopicMessages();

                // Send messages to a Topic!
                await SendTopicMessagesAsync();

                Thread.Sleep(3000);
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Now... closing Topics");
                await topicClient.CloseAsync();
            }
        }

        private static void SubscribeToReeiveTopicMessages()
        {
            //TODO: learn how to properly add FILTERS!!!
            //subscriptionClient.AddRuleAsync(new RuleDescription { Filter = new SqlFilter("From LIKE '%Smith'") });

            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that processes messages.

            SubscriptionClient.RegisterMessageHandler(ProcessTopicMessagesAsync, messageHandlerOptions);
        }

        private static async Task ProcessTopicMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            Console.WriteLine($"Received subscription message::{Encoding.UTF8.GetString(message.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }

        // Use this handler to examine the exceptions received on the message pump.
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        private static async Task SendTopicMessagesAsync()
        {
            const int numberOfMessagesToSend = 4;
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the topic.
                    string messageBody = $"TOPIC Message {i}";
                    var encodedMessage = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending TOPIC message: {messageBody}");

                    // Send the message to the topic.
                    await TopicClient.SendAsync(encodedMessage);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}