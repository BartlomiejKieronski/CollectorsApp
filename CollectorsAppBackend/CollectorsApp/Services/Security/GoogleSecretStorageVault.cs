using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Caching.Memory;

namespace CollectorsApp.Services.Security
{
    public class GoogleSecretStorageVault : IGoogleSecretStorageVault
    {
        private readonly SecretManagerServiceClient _client;
        private readonly IMemoryCache _cache;
        private readonly string _projectId;
        public GoogleSecretStorageVault(IMemoryCache cache)
        {
            _client = SecretManagerServiceClient.Create();
            _cache = cache;
            _projectId = "abiding-splicer-447610-n4";
        }

        public async Task<string> GetSecretsAsync(string secretName)
        {
            if (!_cache.TryGetValue(secretName, out string secretValue))
            {
                try
                {
                    var secretVersion = new SecretVersionName(_projectId, secretName, "latest");
                    
                    AccessSecretVersionResponse result = await _client.AccessSecretVersionAsync(secretVersion);
                    
                    secretValue = result.Payload.Data.ToStringUtf8();
                    
                    _cache.Set(secretName, secretValue);

                }

                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error retrieving secret '{secretName}'", ex);
                }
            }
            return secretValue;
        }

    }
}