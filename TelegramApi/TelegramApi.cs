using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramApi {
    public interface ITelegramApi {
        void SetTelegramBotToken(bool enableTelegramBot, string telegramBotToken);
        Task SendeNachricht(string txt, bool enableNotification);
    }
    

    public class TelegramApi : ITelegramApi {
        private readonly ITelegramPersistenceService _telegramPersistenceService;
        private TelegramBotClient _botClient;
        private List<long> _chatIds;
        private bool _enableTelegramBot;

        public TelegramApi(ITelegramPersistenceService telegramPersistenceService) {
            _telegramPersistenceService = telegramPersistenceService;
        }

        public void SetTelegramBotToken(bool enableTelegramBot, string telegramBotToken) {
            _enableTelegramBot = enableTelegramBot;
            if (!enableTelegramBot) return;

            _botClient = new TelegramBotClient(telegramBotToken);
            _botClient.OnMessage += BotClientOnOnMessage;
            _botClient.StartReceiving();
            _chatIds = _telegramPersistenceService.LadeChatIds();
        }

        public async Task SendeNachricht(string txt, bool enableNotification) {
            if(!_enableTelegramBot) return;

            if(_botClient == null) throw new Exception($"Telegram Bot not initialized");
            foreach (var curChatId in _chatIds) {
                await _botClient.SendTextMessageAsync(curChatId, txt, disableNotification: !enableNotification);
            }
        }

        private async void BotClientOnOnMessage(object sender, MessageEventArgs e) {
            var chatId = e.Message.Chat.Id;
            if (_chatIds.Contains(chatId)) {
                await _botClient.SendTextMessageAsync(chatId, "You are already on the broadcast list");
                return;
            }

            _chatIds.Add(chatId);
            _telegramPersistenceService.SpeichereChatIds(_chatIds);
            await _botClient.SendTextMessageAsync(chatId, "You have been added to the broadcast list");
        }
    }
}