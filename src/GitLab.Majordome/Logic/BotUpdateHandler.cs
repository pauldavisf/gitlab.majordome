using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IBotService _botService;
        private readonly IEnumerable<IBotCommand> botCommands;
        private readonly ILogger<BotUpdateHandler> _logger;

        public BotUpdateHandler(IBotService botService, IEnumerable<IBotCommand> botCommands, ILogger<BotUpdateHandler> logger)
        {
            if (botCommands == null || !botCommands.Any())
            {
                throw new ArgumentException("No bot commands registered");
            }

            _botService = botService;
            this.botCommands = botCommands;
            _logger = logger;
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            _logger.LogInformation("Received Message from {0}", message.Chat.Id);

            foreach (var botCommand in botCommands)
            {
                if (botCommand.CanExecute(message))
                {
                    await botCommand.ExecuteAsync(message);
                    return;
                }
            }

            return;

            switch (message.Type)
            {
                case MessageType.Text:
                    // Echo each Message
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Text);
                    break;

                case MessageType.Photo:
                    // Download Photo
                    var fileId = message.Photo.LastOrDefault()?.FileId;
                    var file = await _botService.Client.GetFileAsync(fileId);

                    var filename = file.FileId + "." + file.FilePath.Split('.').Last();
                    using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                    {
                        await _botService.Client.DownloadFileAsync(file.FilePath, saveImageStream);
                    }

                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
                    break;
            }
        }
    }
}
