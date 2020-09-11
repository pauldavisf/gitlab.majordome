using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitLab.Majordome.Abstractions
{
    public interface IUsersRepository
    {
        IReadOnlyList<User> GetAllUsers();
        string? GetUserEmail(long chatId);
        Task<OperationResult> SaveUserAsync(User user);
        Task SetUserNotifiedDate(string username, DateTime date);
    }
}