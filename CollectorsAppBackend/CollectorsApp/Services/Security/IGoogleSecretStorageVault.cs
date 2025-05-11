namespace CollectorsApp.Services.Security
{
    public interface IGoogleSecretStorageVault
    {
        Task<string> GetSecretsAsync(string SecretName);
    }
}
