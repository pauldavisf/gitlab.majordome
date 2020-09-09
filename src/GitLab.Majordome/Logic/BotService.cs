using GitLab.Majordome.Abstractions;
using GitLab.Majordome.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace GitLab.Majordome.Logic
{
    public class BotService : IBotService
    {
        public BotService(IOptions<Credentials> config)
        {
            Client = new TelegramBotClient(config.Value.BotToken);
        }

        public TelegramBotClient Client { get; }
    }
}
