using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Caching.Memory;

namespace CollectorsApp.Services.Security
{
    /// <summary>
    /// Provides functionality to retrieve secrets from Google Secret Manager with support for in-memory caching.
    /// </summary>
    /// <remarks>This class is designed to interact with Google Secret Manager to securely retrieve secrets. 
    /// It uses an in-memory cache to minimize the number of requests made to the Secret Manager, improving performance 
    /// and reducing latency. The class requires a valid Google Cloud project ID, which should be configured in the 
    /// application's configuration settings under the key "GoogleSecretStorage:ProjectId".</remarks>
    public class GoogleSecretStorageVault : IGoogleSecretStorageVault
    {
        private readonly SecretManagerServiceClient _client;
        private readonly IMemoryCache _cache;
        private readonly string _projectId;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSecretStorageVault"/> class,  providing access to Google
        /// Secret Manager with caching support.
        /// </summary>
        /// <param name="cache">The memory cache used to store secrets for faster retrieval and reduced API calls.</param>
        /// <param name="configuration">The application configuration containing the required settings, including the Google Cloud Project ID.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null.</exception>
        public GoogleSecretStorageVault(IMemoryCache cache, IConfiguration configuration)
        {
            _client = SecretManagerServiceClient.Create();
            _cache = cache;
            _configuration = configuration ??
                throw new ArgumentNullException("No value");
            _projectId = _configuration["GoogleSecretStorage:ProjectId"];
            
        }

        /// <summary>
        /// Retrieves the value of a secret from the secret management system asynchronously.
        /// </summary>
        /// <remarks>If the secret value is not already cached, it is retrieved from the secret management
        /// system and cached for future use.</remarks>
        /// <param name="secretName">The name of the secret to retrieve. This value cannot be null or empty.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the secret
        /// value as a string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if an error occurs while retrieving the secret from the secret management system.</exception>
        public async Task<string> GetSecretsAsync(string secretName)
        {
            if (!_cache.TryGetValue(secretName, out string secretValue))
            {
                try
                {
                    //Define the secret version and project to acces
                    var secretVersion = new SecretVersionName(_projectId, secretName, "latest");
                    //Access the secret version
                    AccessSecretVersionResponse result = await _client.AccessSecretVersionAsync(secretVersion);
                    //Extract the secret payload
                    secretValue = result.Payload.Data.ToStringUtf8();
                    //Cache the secret value
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