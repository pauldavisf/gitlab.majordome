using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace GitLab.Majordome.BotCommands
{
    public interface IBotCommand
    {
        bool CanExecute(Message message);
        Task ExecuteAsync(Message message);
    }
}
