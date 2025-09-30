using CollectorsApp.Services.Security;
using System.Security.Cryptography;
using System.Text;

namespace CollectorsApp.Services.Encryption
{
    /// <summary>
    /// Provides methods for generating and verifying cryptographic hashes using secure algorithms.
    /// </summary>
    /// <remarks>The <see cref="DataHash"/> class includes functionality for generating HMAC hashes, creating
    /// salted hashes, and recreating hashes for verification purposes. It leverages secure cryptographic algorithms
    /// such as HMAC-SHA256 and PBKDF2 to ensure data integrity and security. This class is designed to work with
    /// configuration-based secrets and integrates with a secret storage vault for dynamic secret retrieval.</remarks>
    public class DataHash : IDataHash
    {
        private readonly IConfiguration _configuration;
        private readonly IGoogleSecretStorageVault _vault;
        /// <summary>
        /// Initializes a new instance of the <see cref="DataHash"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration settings. This parameter is used to retrieve configuration values required for
        /// the operation of the <see cref="DataHash"/> instance.</param>
        /// <param name="vault">The Google Secret Storage Vault instance. This parameter is used to securely access and manage secrets
        /// required by the <see cref="DataHash"/> instance.</param>
        public DataHash(IConfiguration configuration, IGoogleSecretStorageVault vault)
        {
            _configuration = configuration;
            _vault = vault;
        }

        /// <summary>
        /// Generates a Hash-based Message Authentication Code (HMAC) for the specified data using a secret key.
        /// </summary>
        /// <remarks>The secret key used for HMAC generation is resolved from the application
        /// configuration or retrieved from a runtime secret vault. This method ensures that the HMAC is computed
        /// securely using the <see cref="System.Security.Cryptography.HMACSHA256"/> algorithm.</remarks>
        /// <param name="data">The input data to be hashed. Cannot be <see langword="null"/> or empty.</param>
        /// <returns>A hexadecimal string representation of the computed HMAC.</returns>
        public async Task<string> GenerateHmacAsync(string data)
        {
            return await Task.Run(async () =>
            {
                // Prefer resolved secret from configuration; fall back to runtime vault call
                var hmacSecret = _configuration["GoogleSecretStorage:Resolved:HMAC"]
                                  ?? await _vault.GetSecretsAsync(_configuration["GoogleSecretStorage:Secrets:HMAC"]);
                byte[] keyBytes = Encoding.UTF8.GetBytes(hmacSecret);
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

        /// <summary>
        /// Asynchronously generates a cryptographic hash of the specified data and returns the hash along with the salt
        /// used.
        /// </summary>
        /// <remarks>The method uses a secure hashing algorithm with a randomly generated salt and a high
        /// number of iterations  to ensure strong cryptographic security. The salt and hash can be used for securely
        /// verifying the data later.</remarks>
        /// <param name="data">The input data to be hashed. Cannot be null or empty.</param>
        /// <returns>A tuple containing the salt as a Base64-encoded string and the cryptographic hash of the input data.</returns>
        public async Task<(string, string)> GetCredentialsAsync(string data)
        {
            byte[] salt = GenerateSalt();
            int iterations = 600000;
            int keyByteLength = 32;
            string dataHash = await CreateDataHashAsync(data, salt, iterations, keyByteLength);
            return (Convert.ToBase64String(salt), dataHash);
        }

        /// <summary>
        /// Recreates a hashed representation of the specified data using the provided salt.
        /// </summary>
        /// <remarks>This method uses a fixed number of iterations and a predefined key length to generate
        /// the hash. The caller is responsible for ensuring that the input data and salt meet the required
        /// conditions.</remarks>
        /// <param name="data">The input data to be hashed. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="salt">The base64-encoded salt to use for hashing. Must be a valid base64 string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the hashed representation of the
        /// input data.</returns>
        public async Task<string> RecreateDataAsync(string data, string salt)
        {
            byte[] byteSalt = Convert.FromBase64String(salt);
            int iterations = 600000;
            int keyByteLength = 32;
            return await CreateDataHashAsync(data, byteSalt, iterations, keyByteLength);
        }

        /// <summary>
        /// Generates a cryptographically secure random salt.
        /// </summary>
        /// <remarks>The generated salt can be used for cryptographic operations such as password
        /// hashing.</remarks>
        /// <returns>A byte array containing 32 cryptographically secure random bytes.</returns>
        public byte[] GenerateSalt()
        {
            var saltBytes = new byte[32];
            RandomNumberGenerator.Fill(saltBytes);
            return saltBytes;
        }

        /// <summary>
        /// Asynchronously generates a cryptographic hash of the specified input data using the PBKDF2 algorithm.
        /// </summary>
        /// <remarks>This method uses the PBKDF2 algorithm with the SHA-256 hash function to derive a
        /// cryptographic key from the input data. The resulting hash is returned as a base64-encoded string.</remarks>
        /// <param name="data">The input data to be hashed. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="salt">The cryptographic salt to use for the hash. Must not be <see langword="null"/>.</param>
        /// <param name="iterations">The number of iterations to perform in the PBKDF2 algorithm. Must be a positive integer.</param>
        /// <param name="keyByteLength">The desired length, in bytes, of the derived key. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the base64-encoded hash string.</returns>
        private static Task<string> CreateDataHashAsync(string data, byte[] salt, int iterations, int keyByteLength)
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