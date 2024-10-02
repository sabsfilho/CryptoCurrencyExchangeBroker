using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceLayerCosmosDBLib
{
    public class CredentialsMap
    {
        const string KV_CONNECTION_STRING = "cosmosdb-connection-string";
        const string KV_URI = "https://skills-belt-kv.vault.azure.net";

        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }

        public CredentialsMap(string? tenantID = null, string? connectionString = null, string? databaseName = null)
        {
            string tID = tenantID ?? "3c614e64-8b9a-45b6-a57d-6c071dfeac0b";
            ConnectionString = connectionString ?? GetConnectionString(tID);
            DatabaseName = databaseName ?? "CryptoCurrencyExchangeBroker";
        }
        private static string GetConnectionString(string tenantID)
        {
            //HACK TO WORK ON LOCAL DEV MACHINE
            /*
            var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions()
            {
                VisualStudioTenantId = tenantID,
                SharedTokenCacheTenantId = tenantID,
                VisualStudioCodeTenantId = tenantID,
                InteractiveBrowserTenantId = tenantID,
                ExcludeManagedIdentityCredential = true
            });
            */
            var credentials = new DefaultAzureCredential();

             var client = new SecretClient(new Uri(KV_URI), credentials);

            var secret = client.GetSecret(KV_CONNECTION_STRING);

            return secret.Value.Value;
            
        }


    }
}
