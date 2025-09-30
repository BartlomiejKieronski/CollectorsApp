using Microsoft.Extensions.Configuration;

namespace CollectorsApp.Services.Security.Configuration
{
    /// <summary>
    /// Represents a configuration source that retrieves configuration data from Google Secret Manager.
    /// </summary>
    /// <remarks>This class is used to integrate Google Secret Manager as a configuration source in an
    /// application. It allows secrets stored in Google Secret Manager to be accessed as configuration values.</remarks>
    public class GoogleSecretManagerConfigurationSource : IConfigurationSource
    {
        private readonly IConfiguration _baseConfiguration;
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSecretManagerConfigurationSource"/> class.
        /// </summary>
        /// <param name="baseConfiguration">The base configuration used to initialize the configuration source. This configuration is typically used to
        /// provide default settings or access credentials required to interact with Google Secret Manager.</param>
        public GoogleSecretManagerConfigurationSource(IConfiguration baseConfiguration)
        {
            _baseConfiguration = baseConfiguration;
        }

        /// <summary>
        /// Builds and returns a configuration provider based on the specified configuration builder.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> used to construct the configuration provider.</param>
        /// <returns>An <see cref="IConfigurationProvider"/> instance that retrieves configuration values.</returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new GoogleSecretManagerConfigurationProvider(_baseConfiguration);
        }
    }
}
