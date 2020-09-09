using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = GitLab.Majordome.Abstractions.User;

namespace GitLab.Majordome.BotCommands
{
    public class EmailCommand : IBotCommand
    {
        private readonly IBotService botService;
        private readonly IUsersRepository usersRepository;

        public EmailCommand(
            IBotService botService,
            IUsersRepository usersRepository)
        {
            this.botService = botService;
            this.usersRepository = usersRepository;
        }

        public bool CanExecute(Message message)
        {
            return message.Type == MessageType.Text && message.Text.StartsWith(@"/email");
        }

        public async Task ExecuteAsync(Message message)
        {
            var messageParts = message.Text.Split(' ');
            if (messageParts.Length != 2)
            {
                await message.ReplyAsync(botService, @"Я вас не понял! После /email нужно написать пробел и ваш адрес");
                return;
            }

            var saveChatResult = await usersRepository.SaveUserAsync(new User
            {
                Email = messageParts[1],
                ChatId = message.Chat.Id,
                TelegramNickname = message.Chat.Username
            });

            if (!saveChatResult.IsSuccess)
            {
                await message.ReplyAsync(botService, $"Я не смог сохранить ваш e-mail! {saveChatResult.Message}");
                return;
            }

            await message.ReplyAsync(botService, "Спасибо, сэр, я сохранил ваши данные");
        }
    }
}