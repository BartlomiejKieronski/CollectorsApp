namespace CollectorsApp.Services.Encryption
{
    public interface IDataHash
    {
        Task<string> GenerateHmacAsync(string data);
        Task<(string, string)> GetCredentialsAsync(string data);
        Task<string> RecreateDataAsync(string data, string salt);
        byte[] GenerateSalt();

    }
}