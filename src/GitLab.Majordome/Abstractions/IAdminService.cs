using System.Threading.Tasks;

namespace GitLab.Majordome.Abstractions
{
    public interface IAdminService
    {
        Task SendToAllUsersAsync(string message);
        Task AddUserAsync(User user);
        Task RefreshKeyboardAsync();
    }
}