using System.Text;

namespace CollectorsApp.Services.Utility
{
    public class WriteFileLog
    {
        public Task WriteTextFileAsync(string content, string filePath = "D://log/file.txt")
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path must not be null or whitespace.", nameof(filePath));

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(filePath) ?? throw new ArgumentException("Invalid file path.", nameof(filePath));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Asynchronously write the content to the file
            File.WriteAllTextAsync(filePath, content, Encoding.UTF8).ConfigureAwait(false);
            return Task.CompletedTask;
            
        }
    }
}
