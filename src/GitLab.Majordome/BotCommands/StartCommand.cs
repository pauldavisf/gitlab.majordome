using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GitLab.Majordome.BotCommands
{
    public class StartCommand : IBotCommand
    {
        private readonly IBotService botService;

        public StartCommand(IBotService botService)
        {
            this.botService = botService;
        }

        public bool CanExecute(Message message)
        {
            return message.Type == MessageType.Text && message.Text.StartsWith(@"/start");
        }

        public async Task ExecuteAsync(Message message)
        {
            await message.ReplyAsync(
                botService,
                "Приветствую, сэр! Я ваш новый дворецкий, буду напоминать о ревью\n" +
                "Чтобы получать сообщения, укажите свой userName, привязанный к GitLab вот так:\n" +
                "/login вашUserName (без @)");
        }
    }
}
