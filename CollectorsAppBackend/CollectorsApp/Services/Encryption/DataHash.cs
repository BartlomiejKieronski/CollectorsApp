using CollectorsApp.Services.Security;
using System.Security.Cryptography;
using System.Text;

namespace CollectorsApp.Services.Encryption
{
    public class DataHash : IDataHash
    {
        private readonly IConfiguration _configuration;
        private readonly IGoogleSecretStorageVault _vault;
        public DataHash(IConfiguration configuration, IGoogleSecretStorageVault vault)
        {
            _configuration = configuration;
            _vault = vault;
        }
        public async Task<string> GenerateHmacAsync(string data)
        {
            return await Task.Run(async () =>
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(await _vault.GetSecretsAsync(_configuration["GoogleSecretStorage:Secrets:HMAC"]));
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                using var hmac = new HMACSHA256(keyBytes);
                byte[] hashBytes = hmac.ComputeHash(dataBytes);

                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
                return sb.ToString();
            });
        }

        public async Task<(string, string)> GetCredentialsAsync(string data)
        {
            byte[] salt = GenerateSalt();
            int iterations = 600000;
            int keyByteLength = 32;
            string dataHash = await CreateDataHashAsync(data, salt, iterations, keyByteLength);
            return (Convert.ToBase64String(salt), dataHash);
        }

        public async Task<string> RecreateDataAsync(string data, string salt)
        {
            byte[] byteSalt = Convert.FromBase64String(salt);
            int iterations = 600000;
            int keyByteLength = 32;
            return await CreateDataHashAsync(data, byteSalt, iterations, keyByteLength);
        }

        public byte[] GenerateSalt()
        {
            var saltBytes = new byte[32];
            RandomNumberGenerator.Fill(saltBytes);
            return saltBytes;
        }

        private Task<string> CreateDataHashAsync(string data, byte[] salt, int iterations, int keyByteLength)
        {
            return Task.Run(() =>
            {
                using (var pbkdf2 = new Rfc2898DeriveBytes(data, salt, iterations, HashAlgorithmName.SHA256))
                {
                    return Convert.ToBase64String(pbkdf2.GetBytes(keyByteLength));
                }
            });
        }
    }
}