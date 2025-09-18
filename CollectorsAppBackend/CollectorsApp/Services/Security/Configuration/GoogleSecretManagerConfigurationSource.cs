using Microsoft.Extensions.Configuration;

namespace CollectorsApp.Services.Security.Configuration
{
    public class GoogleSecretManagerConfigurationSource : IConfigurationSource
    {
        private readonly IConfiguration _baseConfiguration;
        public GoogleSecretManagerConfigurationSource(IConfiguration baseConfiguration)
        {
            _baseConfiguration = baseConfiguration;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new GoogleSecretManagerConfigurationProvider(_baseConfiguration);
        }
    }
}
