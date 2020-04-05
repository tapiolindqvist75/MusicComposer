using Azure.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Storage.Azure;
using System;
using Azure.Security.KeyVault.Secrets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MusicComposer.Web.BL
{
    public class StorageHandler
    {
        private static string GetAccountKey(IConfiguration configuration, IMemoryCache memoryCache)
        {
            if (memoryCache.TryGetValue<string>("storageKey", out string accountKey))
            {
                return accountKey;
            }
            else
            {
                string servicePrincipal = configuration.GetValue<string>("MusicComposerStorage:ServicePrincipal");
                string azureKeyVault = configuration.GetValue<string>("MusicComposerStorage:AzureKeyVaultName");
                string azureKeyVaultKeyName = configuration.GetValue<string>("MusicComposerStorage:AzureKeyVaultKeyName");
                ManagedIdentityCredential credential = new ManagedIdentityCredential(servicePrincipal);
                SecretClient client = new SecretClient(new Uri($"https://{azureKeyVault}.vault.azure.net/"), credential);
                Azure.Response<KeyVaultSecret> response = client.GetSecret(azureKeyVaultKeyName);
                if (response.Value != null && response.Value.Value != null)
                    return response.Value.Value;
            }
            throw new Exception("Failed to retrieve account key from Azure key vault");
        }

        public static IStorageHandler GetStorageHandler(ILogger logger, IConfiguration configuration, IMemoryCache memoryCache)
        {
            try
            {
                string type = configuration.GetValue<string>("MusicComposerStorage:Type");
                if (type == "AzureKeyVault" || type == "AzureStorageAccountKey" || type == "AzureStorageEmulator")
                {
                    AzureStorageParameters parameters = new AzureStorageParameters()
                    {
                        AccountName = configuration.GetValue<string>("MusicComposerStorage:StorageAccountName"),
                    };
                    if (type == "AzureKeyVault")
                        parameters.AccountKey = GetAccountKey(configuration, memoryCache);
                    else if (type == "AzureStorageAccountKey")
                        parameters.AccountKey = configuration.GetValue<string>("MusicComposerStorage:StorageAccountSecret");
                    else
                        parameters.UseEmulator = true;
                    return StorageFactory.GetStorage(parameters);
                }
                throw new NotImplementedException("Storage type invalid or not specified");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving Azure storage information");
                return null;
            }
        }

        public static string ConvertToValidPartitionKey(string value)
        {
            value = value.Replace("/", "");
            value = value.Replace("\\", "");
            value = value.Replace("#", "");
            value = value.Replace("?", "");
            value = value.Replace("\t", "");
            value = value.Replace("\n", "");
            value = value.Replace("\r", "");
            value = value.Replace(".", "");
            value = value.Replace("'", "");
            return value;
        }
    }
}
