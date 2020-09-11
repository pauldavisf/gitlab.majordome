using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using GitLab.Majordome.Configuration;
using GitLab.Majordome.Logic;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GitLab.Majordome.BotCommands
{
    public class ListReviewsCommand : IBotCommand
    {
        private readonly GitLabOptions gitLabOptions;
        private readonly IBotService botService;
        private readonly IUsersRepository usersRepository;
        private readonly IMergeRequestsProvider mergeRequestsProvider;

        public ListReviewsCommand(
            IBotService botService,
            IUsersRepository usersRepository,
            IOptions<GitLabOptions> gitLabOptions,
            IMergeRequestsProvider mergeRequestsProvider)
        {
            this.gitLabOptions = gitLabOptions.Value;
            this.botService = botService;
            this.usersRepository = usersRepository;
            this.mergeRequestsProvider = mergeRequestsProvider;
        }

        public bool CanExecute(Message message)
        {
            return message.Text.StartsWith(@"/list");
        }

        public async Task ExecuteAsync(Message message)
        {
            var username = usersRepository.GetUserEmail(message.Chat.Id);
            if (username == null)
            {
                await message.ReplyAsync(botService, @"Извините, я не нашел ваш e-mail, попробуйте выполнить /email ваша@почта.com");
                return;
            }

            var mergeRequests = await GetMergeRequests(username);
            var reviewsListString = BuildMergeRequestsString(mergeRequests);

            if (reviewsListString.Length > 0)
            {
                await message.ReplyAsync(
                    botService,
                    $"Я нашел следующие ревью без вашего лайка:\n{reviewsListString.EscapeMarkdown()}",
                    ParseMode.MarkdownV2);
            }
            else
            {
                await message.ReplyAsync(botService, "Ничего не нашлось... Похоже, вы проверили все ревью! Мои поздравления");
            }
        }

        private Task<IReadOnlyList<MergeRequestInfo>> GetMergeRequests(string username)
        {
            var getMergeRequestsOptions = new GetMergeRequestsOptionsBuilder()
                .ExcludingProjects(gitLabOptions.ExcludingProjects)
                .NotAuthoredBy(username)
                .NotUpvotedBy(username)
                .OnlyNotWorkInProgress()
                .OnlyOpened()
                .Build();

            return mergeRequestsProvider.GetOpenedMergeRequestAsync(gitLabOptions.ProjectGroupId, getMergeRequestsOptions);
        }

        private static string BuildMergeRequestsString(IReadOnlyList<MergeRequestInfo> mergeRequestInfos)
        {
            var reviewsListStringBuilder = new StringBuilder();
            foreach (var mergeRequest in mergeRequestInfos)
            {
                reviewsListStringBuilder.AppendLine();
                reviewsListStringBuilder.AppendLine($"[{mergeRequest.Title}]({mergeRequest.WebUrl})");
            }

            return reviewsListStringBuilder.ToString();
        }
    }
}