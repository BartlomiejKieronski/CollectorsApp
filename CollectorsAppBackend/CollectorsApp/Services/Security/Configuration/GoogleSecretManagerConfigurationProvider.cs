using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;

namespace CollectorsApp.Services.Security.Configuration
{
    /// <summary>
    /// Loads secrets from Google Secret Manager and overlays them into the configuration tree.
    /// For each mapping under GoogleSecretStorage:Secrets:{key} = {secretName}, this provider fetches
    /// the secret value and writes it back to BOTH:
    ///  - GoogleSecretStorage:Secrets:{key} (so consumers can read normally)
    ///  - GoogleSecretStorage:Resolved:{key} (optional convenience path)
    /// </summary>
    public class GoogleSecretManagerConfigurationProvider : ConfigurationProvider
    {
        private readonly IConfiguration _baseConfiguration;

        public GoogleSecretManagerConfigurationProvider(IConfiguration baseConfiguration)
        {
            _baseConfiguration = baseConfiguration ?? throw new ArgumentNullException(nameof(baseConfiguration));
        }

        public override void Load()
        {
            var projectId = _baseConfiguration["GoogleSecretStorage:ProjectId"];            
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return; // Nothing to do without project id
            }

            var secretsSection = _baseConfiguration.GetSection("GoogleSecretStorage:Secrets");
            if (!secretsSection.Exists())
            {
                return; // No mappings defined
            }

            var client = SecretManagerServiceClient.Create();

            foreach (var child in secretsSection.GetChildren())
            {
                var key = child.Key;               // e.g. DB-STRING
                var secretName = child.Value;       // e.g. actual GSM secret id
                if (string.IsNullOrWhiteSpace(secretName))
                    continue;

                try
                {
                    var secretVersion = new SecretVersionName(projectId, secretName, "latest");
                    var result = client.AccessSecretVersion(secretVersion);
                    var secretValue = result.Payload.Data.ToStringUtf8();

                    // Overwrite the secrets mapping with resolved values for easy consumption
                    Data[$"GoogleSecretStorage:Secrets:{key}"] = secretValue;
                    Data[$"GoogleSecretStorage:Resolved:{key}"] = secretValue;
                }
                catch
                {
                }
            }
        }
    }
}
