﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramApi {
    public interface ITelegramApi {
        Task SendeNachricht(string txt);
    }

    public class TelegramApi : ITelegramApi {
        private readonly ITelegramPersistenceService _telegramPersistenceService;
        private readonly TelegramBotClient _botClient;
        private readonly List<long> _chatIds;

        public TelegramApi(ITelegramPersistenceService telegramPersistenceService) {
            _telegramPersistenceService = telegramPersistenceService;
            _botClient = new TelegramBotClient("***REMOVED***");
            _botClient.OnMessage += BotClientOnOnMessage;
            _botClient.StartReceiving();
            _chatIds = _telegramPersistenceService.LadeChatIds();
        }

        private async void BotClientOnOnMessage(object sender, MessageEventArgs e) {
            var chatId = e.Message.Chat.Id;
            if (_chatIds.Contains(chatId)) {
                await _botClient.SendTextMessageAsync(chatId, "Du bist bereits in der Broadcast-Liste");
                return;
            }

            _chatIds.Add(chatId);
            _telegramPersistenceService.SpeichereChatIds(_chatIds);
            await _botClient.SendTextMessageAsync(chatId, "Du wurdest zur Broadcast-Liste hinzugefügt");
        }

        public async Task SendeNachricht(string txt) {
            foreach (var curChatId in _chatIds) {
                await _botClient.SendTextMessageAsync(curChatId, txt);
            }
        }
    }
}