using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ServiceBus
{
    public static class ActiveDirectoryService
    {
        public static string ServiceBusConnectionString { get; set; }

        public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authoContext = new AuthenticationContext(authority);

            //Get from Configuration (local) the CLient ID (of the registered App who has access)
            //Get from Configuration (local) the Client Secret (of the registered App who has access)

            ClientCredential clientCred = new ClientCredential(Configuration.ClientId, Configuration.ClientSecret);

            var result = await authoContext.AcquireTokenAsync(resource, clientCred);
            if (result == null)
                throw new InvalidOperationException("Failed to obatin JWT");

            return result.AccessToken;
        }
    }
}