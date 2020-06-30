using Microsoft.Azure.ServiceBus;

namespace ServiceBus
{
    public static class QueueHandler
    {
        public static IQueueClient CreateQueClient(string serviceBusConnectionString, string queueName)
        {
            return new QueueClient(serviceBusConnectionString, queueName);
        }
    }
}