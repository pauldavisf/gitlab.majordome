using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using GitLab.Majordome.BotCommands;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GitLab.Majordome.Logic
{
    public class BotUpdateHandler : IBotUpdateHandler
    {
        private readonly IEnumerable<IBotCommand> botCommands;
        private readonly ILogger<BotUpdateHandler> logger;

        public BotUpdateHandler(IEnumerable<IBotCommand> botCommands, ILogger<BotUpdateHandler> logger)
        {
            if (botCommands == null || !botCommands.Any())
            {
                throw new ArgumentException("No bot commands registered");
            }

            this.botCommands = botCommands;
            this.logger = logger;
        }

        public async Task HandleAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            logger.LogInformation("Received Message from {0}", message.Chat.Id);

            foreach (var botCommand in botCommands)
            {
                if (!botCommand.CanExecute(message))
                {
                    continue;
                }

                await botCommand.ExecuteAsync(message);
                return;
            }
        }
    }
}
