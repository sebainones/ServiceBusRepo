﻿using Microsoft.Extensions.Configuration;

namespace ServiceBus
{
    public class Configuration
    {
        private static readonly IConfigurationBuilder configBuilder = new ConfigurationBuilder().AddJsonFile("localsettings.json");
        public static string ServiceBusConnectionString;
        public static string QueueName;
        public static string TopicName;
        public static string SubscriptionName;

        static Configuration()
        {            
            IConfigurationRoot config = configBuilder.Build();
            ServiceBusConnectionString = config["ServiceBusConnectionString"];
            QueueName = config["QueueName"];
            TopicName = config["TopicName"];
            SubscriptionName = config["SubscriptionName"];
        }
    }
}