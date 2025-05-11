using System.Security.Cryptography;

namespace CollectorsApp.Services.Utility
{
    public class GenerateUniqueDeviceId
    {
        public async Task<string> DeviceId()
        {
            var bytes = new byte[16];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}