using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FacebookEventParser.OutputServices
{
    public interface IFileWriter {
        Task WriteToFile(string txtToWrite, string filenameWithExtension);
    }

    public class FileWriter : IFileWriter {
        private readonly ILogger<FileWriter> _logger;
        private readonly string _folderPath;

        public FileWriter(ILogger<FileWriter> logger) {
            _logger = logger;
            var currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _folderPath = currentDir;
        }

        public async Task WriteToFile(string txtToWrite, string filenameWithExtension) {
            var filePath = Path.Combine(_folderPath, filenameWithExtension);

            _logger.LogInformation($"Writing Content to '{filePath}'");
            
            if(File.Exists(filePath)) File.Delete(filePath);
            await File.WriteAllTextAsync(filePath, txtToWrite);

            _logger.LogInformation("Done");
        }
    }
}
