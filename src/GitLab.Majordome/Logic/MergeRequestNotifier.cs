using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return Task.Factory.StartNew(NotifyAllUsersAsync, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task NotifyAllUsersAsync()
        {
            while (true)
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

                await Task.Delay(TimeSpan.FromMinutes(2)); // TODO: вынести в настройки
            }
        }

        private async Task NotifyUserAsync(User user, IReadOnlyList<MergeRequestInfo> mergeRequests)
        {
            if (DateTime.UtcNow - user.LastNotifyDate < TimeSpan.FromHours(24)
                || DateTime.UtcNow.TimeOfDay > new TimeSpan(22, 0, 0)
                && DateTime.UtcNow.TimeOfDay < new TimeSpan(9, 0, 0))
            {
                return;
            }

            var mergeRequestsToNotify = GetNotUpvotedByUser(mergeRequests, user.Email);

            await SendMessageAsync(user, mergeRequestsToNotify);
        }

        private async Task SendMessageAsync(User user, IReadOnlyList<MergeRequestInfo> mergeRequests)
        {
            if (mergeRequests.Count == 1)
            {
                await botService.Client.SendTextMessageAsync(
                    user.ChatId,
                    $"Сэр, напоминаю вам о [ревью \"{mergeRequests[0].Title}\"]({mergeRequests[0].WebUrl}), ".EscapeMarkdown() +
                    "не вижу вашего лайка",
                    ParseMode.MarkdownV2);
            }
            else if (mergeRequests.Count > 1)
            {
                var requestsString = BuildMergeRequestsString(mergeRequests).EscapeMarkdown();

                await botService.Client.SendTextMessageAsync(
                    user.ChatId,
                    $"Я нашел ревью без вашего лайка:\n{requestsString}",
                    ParseMode.MarkdownV2);
            }

            await usersRepository.SetUserNotifiedDate(user.Email, DateTime.UtcNow);
        }

        private static string BuildMergeRequestsString(IReadOnlyList<MergeRequestInfo> mergeRequests)
        {
            var reviewsListStringBuilder = new StringBuilder();
            foreach (var mergeRequest in mergeRequests)
            {
                reviewsListStringBuilder.AppendLine();
                reviewsListStringBuilder.AppendLine($"[{mergeRequest.Title}]({mergeRequest.WebUrl})");
            }

            return reviewsListStringBuilder.ToString();
        }

        private static IReadOnlyList<MergeRequestInfo> GetNotUpvotedByUser(IReadOnlyList<MergeRequestInfo> mergeRequests, string username)
        {
            return mergeRequests.Where(x =>
                    x.AuthorUsername != username
                    && x.UpvotedBy
                        .All(upvoteUsername => upvoteUsername != username))
                .ToList();
        }
    }
}