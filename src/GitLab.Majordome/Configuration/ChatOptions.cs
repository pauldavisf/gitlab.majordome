using System.Collections.Generic;
using GitLab.Majordome.Abstractions;

namespace GitLab.Majordome.Configuration
{
    public class ChatOptions
    {
        public IList<User> Users { get; set; }
    }
}