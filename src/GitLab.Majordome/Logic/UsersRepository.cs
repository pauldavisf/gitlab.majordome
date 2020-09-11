using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using GitLab.Majordome.Configuration;
using Microsoft.Extensions.Options;

namespace GitLab.Majordome.Logic
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ChatOptions chatOptions;

        public UsersRepository(IOptions<ChatOptions> chatOptions)
        {
            this.chatOptions = chatOptions.Value;
        }

        public IReadOnlyList<User> GetAllUsers()
        {
            return chatOptions.Users.ToList();
        }

        public string? GetUserEmail(long chatId)
        {
            return chatOptions.Users.FirstOrDefault(x => x.ChatId == chatId)?.Email;
        }

        public async Task<OperationResult> SaveUserAsync(User user)
        {
            // if (!IsEmailCorrect(user.Email))
            // {
            //     return OperationResult.Error(ErrorType.IncorrectData, "Email некорректен");
            // }

            if (chatOptions.Users.Any(x => x.Email == user.Email.ToLowerInvariant()))
            {
                return OperationResult.Ok();
            }

            var existingUser = chatOptions.Users.FirstOrDefault(x => x.ChatId == user.ChatId);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;
                existingUser.TelegramNickname = user.TelegramNickname;
            }
            else
            {
                chatOptions.Users.Add(user);
            }

            await SaveOptionsToFile();

            return OperationResult.Ok();
        }

        public async Task SetUserNotifiedDate(string username, DateTime date)
        {
            var user = chatOptions.Users.FirstOrDefault(x => x.Email == username);
            if (user != null)
            {
                user.LastNotifyDate = date;
                await SaveOptionsToFile();
            }
        }

        private async Task SaveOptionsToFile()
        {
            var serializedOptions = JsonSerializer.Serialize(
                chatOptions,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            await File.WriteAllTextAsync("chatSettings.json", serializedOptions);
        }

        private static bool IsEmailCorrect(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            try
            {
                var _ = new MailAddress(email);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }
    }
}