using Telegram.Bot;

namespace GitLab.Majordome.Abstractions
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}