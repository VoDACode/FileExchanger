using Core;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileExchanger.Telegram.Handlers
{
    public abstract class Handler
    {
        public abstract string Command { get; }
        private ITelegramBotClient _botClient;
        private Update _update;
        protected ITelegramBotClient BotClient => _botClient;
        protected Update Update => _update;
        protected DbApp DB => new DbApp(Config.Instance.DbConnect);
        protected long UserId => Update.Message!.From.Id;
        protected long ChatId => Update.Message.Chat.Id;
        protected abstract void Execute();
        public void Invoce(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _update = update;
            Execute();
        }
        
    }
}
