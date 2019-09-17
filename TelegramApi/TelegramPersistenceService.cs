using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace TelegramApi {
    public interface ITelegramPersistenceService {
        void SpeichereChatIds(IEnumerable<long> chatIDs);
        List<long> LadeChatIds();
    }

    public class TelegramPersistenceService : ITelegramPersistenceService {
        private readonly string _filePath;

        public TelegramPersistenceService() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FacebookEventParser", "chatIds.conf");
                var folder = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            }
            else {
                _filePath = Path.Combine("chatIds.conf");
            }
        }
        public void SpeichereChatIds(IEnumerable<long> chatIDs) {
            var content = string.Join(Environment.NewLine, chatIDs);
            if (File.Exists(_filePath)) {
                File.Delete(_filePath);
            }

            File.WriteAllText(_filePath, content);
        }

        public List<long> LadeChatIds() {
            if (!File.Exists(_filePath)) return new List<long>(0);

            var content = File.ReadAllText(_filePath);
            var splitStr = content.Split(Environment.NewLine);
            return splitStr.Select(x => Convert.ToInt64(x)).ToList();
        }
    }
}