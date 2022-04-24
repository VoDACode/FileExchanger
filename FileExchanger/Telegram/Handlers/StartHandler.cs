using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileExchanger.Telegram.Handlers
{
    public class StartHandler : Handler
    {
        public override string Command => "/start";

        protected override async void Execute()
        {
            using (var db = DB)
            {
                var user = db.TelegramUsers.SingleOrDefault(p => p.TelegramId == UserId);
                if (user == null)
                {
                    user = db.TelegramUsers.Add(new Models.TelegramUserModel()
                    {
                        AuthKey = "".RandomString(128),
                        TelegramId = UserId,
                        ChatId = ChatId,
                        IsAuth = false
                    }).Entity;
                    db.SaveChanges();
                }
                else if (user.IsAuth)
                    return;

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Authorization", $"{Config.Instance.MainHost}c/telegram?code={user.AuthKey}")
                    },
                });

                await BotClient.SendTextMessageAsync(
                    chatId: ChatId,
                    text: $"Hi! Are you should authorization.",
                    replyMarkup: inlineKeyboard,
                    parseMode: ParseMode.Markdown, disableWebPagePreview: true);
            }
        }
    }
}
