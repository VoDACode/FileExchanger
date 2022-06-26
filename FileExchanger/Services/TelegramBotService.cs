using Core.Models;
using FileExchanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileExchanger.Services
{
    public delegate void AuthEvent(ITelegramBotClient botClient, Update update, bool isAuth, int userId);
    public class TelegramBotService
    {
        private static TelegramBotService instance;
        public static TelegramBotService Instance => instance ?? (instance = new TelegramBotService());
        private List<Telegram.Handlers.Handler> handlers = new List<Telegram.Handlers.Handler>();
        private TelegramBotClient bot { get; set; }
        public ITelegramBotClient Bot => bot;
        private bool _isRuning = false;
        public string Username => Bot.GetMeAsync().Result.Username;
        private List<long> keys = new List<long>();
        private TelegramBotService()
        {
            Config.Instance.OnUpdata += Config_OnUpdata;
            handlers.Add(new Telegram.Handlers.StartHandler());
        }
        public event AuthEvent OnAuth;

        public void Start()
        {
            if (_isRuning)
            {
                return;
            }
            try
            {
                bot = new TelegramBotClient(Config.Instance.Security.Telegram.Token);
                var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { }, // receive all update types
                };
                bot.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken
                );
                _isRuning = true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async void SendConfirmLogin(TelegramUserModel userModel, Dictionary<string, string> info)
        {
            var ticks = DateTime.Now.Ticks;
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Yes", $"{userModel.Id},auth,true,{ticks}"),
                    InlineKeyboardButton.WithCallbackData("No", $"{userModel.Id},auth,false,{ticks}")
                },
            });
            var text = $"Someone wants to sign in to your account.\n\n";
            foreach(var item in info)
            {
                text += $"{item.Key}: {item.Value}\n";
            }
            text += "\nAllow entry?";
            await Bot.SendTextMessageAsync(
                chatId: userModel.ChatId,
                text: text,
                replyMarkup: inlineKeyboard);
            keys.Add(ticks);
        }

        private void Config_OnUpdata()
        {
            Start();
        }
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == UpdateType.CallbackQuery)
            {
                await callbackButton(botClient, update, cancellationToken);
                return;
            }
            if (update.Type != UpdateType.Message)
                return;
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            var h = handlers.SingleOrDefault(p => p.Command == update.Message.Text);
            if (h == null)
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Incorrect command!", cancellationToken: cancellationToken);
            else
                h.Invoce(botClient, update, cancellationToken);
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task callbackButton(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var parseData = update.CallbackQuery.Data.Split(',');
            var chatId = update.CallbackQuery.Message.Chat.Id;
            var messageId = update.CallbackQuery.Message.MessageId;
            var messageText = update.CallbackQuery.Message.Text;
            if (parseData[1] == "auth")
            {
                IReplyMarkup inlineKeyboard = new ReplyKeyboardRemove();
                await botClient.EditMessageReplyMarkupAsync(chatId, messageId);
                await botClient.EditMessageTextAsync(chatId: chatId,
                    messageId: messageId,
                    messageText + $"\n\n**Login {(parseData[2] == "true" ? "allowed" : "denied")}**",
                    parseMode: ParseMode.Markdown);
                keys.Remove(long.Parse(parseData[3]));
                OnAuth?.Invoke(botClient, update, parseData[2] == "true", int.Parse(parseData[0]));
            }
        }
    }
}
