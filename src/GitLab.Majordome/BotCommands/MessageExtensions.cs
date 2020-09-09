using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GitLab.Majordome.BotCommands
{
    internal static class MessageExtensions
    {
        public static async Task ReplyAsync(this Message message, IBotService botService, string text, ParseMode parseMode = ParseMode.Default)
        {
            await botService.Client.SendTextMessageAsync(message.Chat.Id, text, parseMode);
        }
    }
}