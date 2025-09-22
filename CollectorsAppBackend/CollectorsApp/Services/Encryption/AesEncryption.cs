using CollectorsApp.Services.Security;
using System.Security.Cryptography;
using System.Text;

namespace CollectorsApp.Services.Encryption
{
    public class AesEncryption : IAesEncryption
    {
        private readonly IConfiguration _configuration;
        private readonly IGoogleSecretStorageVault _vault;
        /// <summary>
        /// Initializes a new instance of the <see cref="AesEncryption"/> class,
        /// providing services for AES encryption and decryption using the AES algorithm.
        /// </summary>
        /// <param name="configuration">The application configuration for retrieving settings such as the AES key.</param>
        /// <param name="vault">The secret manager used to securely fetch secrets if not available in configuration.</param>
        public AesEncryption(IConfiguration configuration, IGoogleSecretStorageVault vault)
        {
            _configuration = configuration;
            _vault = vault;
        }

        /// <summary>
        /// Generates a random 16-byte initialization vector (IV) for AES encryption.
        /// </summary>
        /// <returns>A 16-byte array containing a randomly generated initialization vector (IV).</returns>
        public byte[] GenerateIVBytes()
        {
            byte[] iv = new byte[16];
            RandomNumberGenerator.Fill(iv);
            return iv;
        }

        /// <summary>
        /// Encrypts the provided data using AES encryption in CBC mode with PKCS7 padding.
        /// </summary>
        /// <param name="data">The plaintext string to encrypt.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item>Item1: The AES-encrypted data in Base64 format.</item>
        /// <item>Item2: The initialization vector (IV) used, in Base64 format.</item>
        /// </list>
        /// If the input <paramref name="data"/> is null, returns ("No data passed", "").
        /// </returns>
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

        /// <summary>
        /// Decrypts the provided AES-encrypted data using the given initialization vector (IV).
        /// </summary>
        /// <param name="data">AES-encrypted data in Base64 format</param>
        /// <param name="IVKey">Initialization vector (IV) in Base64 format</param>
        /// <returns>The decrypted string, or "No data passed" if the input data is null.</returns>
        public async Task<string> AesDecrypt(string data, string IVKey)
        {
            if (data == null)
                return "No data passed";

            var keyBase64 = _configuration["GoogleSecretStorage:Resolved:AES-KEY"]
                 ?? await _vault.GetSecretsAsync(_configuration["GoogleSecretStorage:Secrets:AES-KEY"]);

            var cipherBytes = Convert.FromBase64String(data);
            var ivBytes = Convert.FromBase64String(IVKey);

            return await Task.Run(() =>
            {
                using var aes = Aes.Create();
                aes.Key = Convert.FromBase64String(keyBase64);
                aes.IV = ivBytes;
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