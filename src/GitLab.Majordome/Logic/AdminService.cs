using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;

namespace GitLab.Majordome.Logic
{
    class AdminService : IAdminService
    {
        private readonly IBotService botService;
        private readonly IUsersRepository usersRepository;

        public AdminService(
            IBotService botService,
            IUsersRepository usersRepository)
        {
            this.botService = botService;
            this.usersRepository = usersRepository;
        }

        public async Task SendToAllUsersAsync(string message)
        {
            var users = usersRepository.GetAllUsers();

            foreach (var user in users)
            {
                await botService.Client.SendTextMessageAsync(user.ChatId, message);
            }
        }

        public Task AddUserAsync(User user)
        {
            usersRepository.SaveUserAsync(user);

            return Task.CompletedTask;
        }

        public async Task RefreshKeyboardAsync()
        {
            var users = usersRepository.GetAllUsers();

            foreach (var user in users)
            {
                await botService.Client.SendTextMessageAsync(user.ChatId, null, replyMarkup: Keyboards.KeyboardMarkup);
            }
        }
    }
}