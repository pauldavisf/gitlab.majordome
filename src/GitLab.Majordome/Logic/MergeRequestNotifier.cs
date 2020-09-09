using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using GitLab.Majordome.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.Enums;

namespace GitLab.Majordome.Logic
{
    public class MergeRequestNotifier : IHostedService
    {
        private readonly GitLabOptions gitLabOptions;
        private readonly IMergeRequestsProvider mergeRequestsProvider;
        private readonly IUsersRepository usersRepository;
        private readonly IBotService botService;
        private Timer? timer;

        public MergeRequestNotifier(
            IMergeRequestsProvider mergeRequestsProvider,
            IOptions<GitLabOptions> gitLabOptions,
            IUsersRepository usersRepository,
            IBotService botService)
        {
            this.gitLabOptions = gitLabOptions.Value;
            this.mergeRequestsProvider = mergeRequestsProvider;
            this.usersRepository = usersRepository;
            this.botService = botService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(NotifyAllUsersAsync!, cancellationToken, TimeSpan.Zero, TimeSpan.FromSeconds(3));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async void NotifyAllUsersAsync(object state)
        {
            var getMergeRequestsOptions = new GetMergeRequestsOptionsBuilder()
                .ExcludingProjects(gitLabOptions.ExcludingProjects)
                .OnlyOpened()
                .OnlyNotWorkInProgress()
                .Build();

            var mergeRequests = await mergeRequestsProvider.GetOpenedMergeRequestAsync(
                gitLabOptions.ProjectGroupId,
                getMergeRequestsOptions);

            var users = usersRepository.GetAllUsers();
            var notifyTasks = users.Select(async user => await NotifyUserAsync(user, mergeRequests));
            await Task.WhenAll(notifyTasks);
        }

        private async Task NotifyUserAsync(User user, IList<MergeRequestInfo> mergeRequests)
        {
            if (DateTime.UtcNow - user.LastNotifyDate < TimeSpan.FromHours(24)
                || DateTime.UtcNow.TimeOfDay > new TimeSpan(22, 0, 0)
                && DateTime.UtcNow.TimeOfDay < new TimeSpan(6, 0, 0))
            {
                return;
            }

            var mergeRequestsToNotify = GetNotUpvotedByUser(mergeRequests, user.Email);

            var notifyTasks = mergeRequestsToNotify.Select(async mergeRequest =>
            {
                await botService.Client.SendTextMessageAsync(
                    user.ChatId,
                    $"Сэр, напоминаю вам о [ревью \"{mergeRequest.Title}\"]({mergeRequest.WebUrl}), ".EscapeMarkdown() +
                    "так как все еще не вижу вашего лайка",
                    ParseMode.MarkdownV2);

                await usersRepository.SetUserNotifiedDate(user.Email, DateTime.UtcNow);
            });

            await Task.WhenAll(notifyTasks);
        }

        public static IList<MergeRequestInfo> GetNotUpvotedByUser(IList<MergeRequestInfo> mergeRequests, string username)
        {
            return mergeRequests.Where(x =>
                    x.AuthorUsername != username
                    && x.UpvotedBy
                        .All(upvoteUsername => upvoteUsername != username))
                .ToList();
        }
    }
}