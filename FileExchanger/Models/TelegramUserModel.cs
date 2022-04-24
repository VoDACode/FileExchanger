namespace FileExchanger.Models
{
    public class TelegramUserModel
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public long ChatId { get; set; }
        public bool IsAuth { get; set; } = false;
        public string AuthKey { get; set; }
        public AuthClientModel AuthClient { get; set; }
    }
}
