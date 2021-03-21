using System;
using System.Threading.Tasks;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace TrainSchedule.Config
{
    public class CosmosTableDB
    {
#if DEBUG
        private static string _connectionString = "testtrainschedulecosmosCS";
#else
        private static string _connectionString = "trainschedulecosmosCS";
#endif

        /// <summary>
        /// Get the DB connection string from Azure Vault
        /// </summary>
        public static string GetConnectionString()
        {
            return getSecret(_connectionString).Result;
        }

        private static async Task<string> getSecret(string secretName)
        {
            var kvUri = $"https://trainschedulevault.vault.azure.net/";
            SecretClient client = new SecretClient(new Uri(kvUri), new AzureCliCredential());

            var secret = await client.GetSecretAsync(secretName);

            return secret.Value.Value;
        }
    }
}
