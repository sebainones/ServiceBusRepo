using Microsoft.Extensions.Configuration;

namespace ServiceBus
{
    public class Configuration
    {
        private static readonly IConfigurationBuilder configBuilder = new ConfigurationBuilder().AddJsonFile("localsettings.json");
        public static string ServiceBusConnectionString;
        public static string QueueName;
        public static string TopicName;
        public static string SubscriptionName;

        //ClientId who has access permisssions to the Key Vault Secrets!
        public static string ClientId {get;private set;}
        
        //Client secrets: A secret string that the application uses to prove its identity when requesting a token. Also can be referred to as application password.
        public static string ClientSecret {get;private set;}

        //URI of the specific secret we are trying to  retrieve.
        public static string SecretUri {get;private set;}

        static Configuration()
        {            
            IConfigurationRoot config = configBuilder.Build();
            ServiceBusConnectionString = config["ServiceBusConnectionString"];
            QueueName = config["QueueName"];
            TopicName = config["TopicName"];
            SubscriptionName = config["SubscriptionName"];

            ClientId = config["ClientId"];
            ClientSecret= config["ClientSecret"];
            SecretUri = config["SecretUri"];
        }
    }
}