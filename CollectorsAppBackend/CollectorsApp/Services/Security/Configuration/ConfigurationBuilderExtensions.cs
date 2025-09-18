using Microsoft.Extensions.Configuration;

namespace CollectorsApp.Services.Security.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddGoogleSecretManager(this IConfigurationBuilder builder)
        {
            // Build a temporary configuration to read ProjectId and mappings from appsettings/env vars only.
            var tempConfig = builder.Build();
            builder.Add(new GoogleSecretManagerConfigurationSource(tempConfig));
            return builder;
        }
    }
}
