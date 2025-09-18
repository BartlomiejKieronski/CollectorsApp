using CollectorsApp.Services.Security;
using System.Security.Cryptography;
using System.Text;

namespace CollectorsApp.Services.Encryption
{
    public class AesEncryption : IAesEncryption
    {
        private readonly IConfiguration _configuration;
        private readonly IGoogleSecretStorageVault _vault;
        public AesEncryption(IConfiguration configuration, IGoogleSecretStorageVault vault)
        {
            _configuration = configuration;
            _vault = vault;
        }
        public byte[] GenerateIVBytes()
        {
            byte[] iv = new byte[16];
            RandomNumberGenerator.Fill(iv);
            return iv;
        }

        public async Task<(string, string)> AesEncrypt(string data)
        {
            if (data == null)
                return ("No data passed", "");
            return await Task.Run(async () =>
            {
                using var aes = Aes.Create();

                var keyBase64 = _configuration["GoogleSecretStorage:Resolved:AES-KEY"]
                                 ?? await _vault.GetSecretsAsync(_configuration["GoogleSecretStorage:Secrets:AES-KEY"]);
                aes.Key = Convert.FromBase64String(keyBase64);
                aes.IV = GenerateIVBytes();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs, Encoding.UTF8))
                {
                    writer.Write(data);
                }
                var encryptedBytes = ms.ToArray();

                return (Convert.ToBase64String(encryptedBytes), Convert.ToBase64String(aes.IV));
            });
        }
        public async Task<string> AesDecrypt(string data, string IVKey)
        {
            if (data == null)
                return "No data passed";

            return await Task.Run(async() =>
            {
                var cipherBytes = Convert.FromBase64String(data);

                using var aes = Aes.Create();
                //aes.Key = Convert.FromBase64String(await _vault.GetSecretsAsync(_configuration["GoogleSecretStorage:Secrets:AES-KEY"]));
                var keyBase64 = _configuration["GoogleSecretStorage:Resolved:AES-KEY"]
                                 ?? await _vault.GetSecretsAsync(_configuration["GoogleSecretStorage:Secrets:AES-KEY"]);
                aes.Key = Convert.FromBase64String(keyBase64);

                aes.IV = Convert.FromBase64String(IVKey);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(cipherBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs, Encoding.UTF8);
                return reader.ReadToEnd();
            });
        }
    }
}