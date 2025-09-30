using System.Security.Cryptography;

namespace CollectorsApp.Services.Utility
{
    /// <summary>
    /// Provides functionality to generate a unique device identifier.
    /// </summary>
    /// <remarks>The generated identifier is a Base64-encoded string representing a 128-bit (16-byte) random
    /// value. This can be used for scenarios requiring unique device identification, such as tracking or
    /// authentication.</remarks>
    public class GenerateUniqueDeviceId
    {
        /// <summary>
        /// Generates a unique device identifier as a Base64-encoded string.
        /// </summary>
        /// <remarks>The generated identifier is a 16-byte random value encoded as a Base64 string.  This
        /// method is suitable for scenarios where a unique, non-persistent identifier is required.</remarks>
        /// <returns>A Base64-encoded string representing a randomly generated 16-byte device identifier.</returns>
        public Task<string> DeviceId()
        {
            var bytes = new byte[16];
            RandomNumberGenerator.Fill(bytes);
            return Task.FromResult(Convert.ToBase64String(bytes));
        }
    }
}