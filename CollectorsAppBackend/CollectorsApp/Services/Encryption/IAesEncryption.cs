namespace CollectorsApp.Services.Encryption
{
    public interface IAesEncryption
    {
        byte[] GenerateIVBytes();
        Task<(string, string)> AesEncrypt(string data);
        Task<string> AesDecrypt(string data, string IVKey);
    }
}