using System;
using System.Threading;
using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace GitLab.Majordome.Logic
{
    public class PollingService : IHostedService
    {
        private readonly IBotService botService;
        private readonly IBotUpdateHandler updateHandler;

        public PollingService(
            IBotService botService,
            IBotUpdateHandler updateHandler)
        {
            this.botService = botService;
            this.updateHandler = updateHandler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            botService.Client.DeleteWebhookAsync();
            botService.Client.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await updateHandler.HandleAsync(update);
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);

            return Task.CompletedTask;
        }
    }
}