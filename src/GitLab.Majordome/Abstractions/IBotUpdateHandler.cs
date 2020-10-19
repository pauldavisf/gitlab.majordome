using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace GitLab.Majordome.Abstractions
{
    public interface IBotUpdateHandler
    {
        Task HandleAsync(Update update);
    }
}
