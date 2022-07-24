using Azure.Messaging.ServiceBus;
using System;

namespace ServiceBus
{
    public static class QueueHandler
    {
        // Create the clients that we'll use for sending and processing messages.
        public static ServiceBusClient GetClient(string serviceBusConnectionString)
        {
            return new ServiceBusClient(serviceBusConnectionString);
        }

        public static ServiceBusSender GetSender(ServiceBusClient client, string queueName)
        {
            return client.CreateSender(queueName);
        }
    }
}