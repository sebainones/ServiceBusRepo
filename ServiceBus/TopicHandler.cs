using Microsoft.Azure.ServiceBus;

namespace ServiceBus
{
    public static class TopicHandler
    {

        public static ITopicClient CreatTopicClient(string serviceBusConnectionString, string topicName)
        {
            return new TopicClient(serviceBusConnectionString, topicName);
        }
    }
}
