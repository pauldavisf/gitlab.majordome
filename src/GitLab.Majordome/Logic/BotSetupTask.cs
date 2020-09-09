using System.Threading;
using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using GitLab.Majordome.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GitLab.Majordome.Logic
{
    public class BotSetupTask : IHostedService
    {
        private readonly IBotService botService;
        private readonly BotOptions botOptions;

        public BotSetupTask(IBotService botService, IOptions<BotOptions> botOptions)
        {
            this.botService = botService;
            this.botOptions = botOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await botService.Client.SetWebhookAsync(botOptions.WebHookPath, cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}