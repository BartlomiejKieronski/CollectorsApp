using Microsoft.Extensions.Configuration;

namespace CollectorsApp.Services.Security.Configuration
{
    /// <summary>
    /// Provides extension methods for adding Google Secret Manager as a configuration source to an <see
    /// cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds Google Secret Manager as a configuration source to the specified <see cref="IConfigurationBuilder"/>.
        /// </summary>
        /// <remarks>This method initializes a temporary configuration to read the necessary settings,
        /// such as the Google Cloud Project ID,  and then adds a configuration source that retrieves secrets from
        /// Google Secret Manager. Ensure that the required Google Cloud credentials and configuration settings are
        /// properly set up before using this method.</remarks>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to which the Google Secret Manager configuration source will be
        /// added.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/> with the Google Secret Manager configuration source added.</returns>
        public static IConfigurationBuilder AddGoogleSecretManager(this IConfigurationBuilder builder)
        {
            // Build a temporary configuration to read ProjectId and mappings from appsettings/env vars only.
            var tempConfig = builder.Build();
            builder.Add(new GoogleSecretManagerConfigurationSource(tempConfig));
            return builder;
        }
    }
}
