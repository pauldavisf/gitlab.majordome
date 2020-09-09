using System;

namespace GitLab.Majordome.Abstractions
{
    public class User
    {
        public long ChatId { get; set; }
        public string Email { get; set; } = default!;
        public string? TelegramNickname { get; set; }
        public DateTime? LastNotifyDate { get; set; }
    }
}