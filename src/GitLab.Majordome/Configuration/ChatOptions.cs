using System;
using System.Collections.Generic;
using GitLab.Majordome.Abstractions;

namespace GitLab.Majordome.Configuration
{
    public class ChatOptions
    {
        public IReadOnlyList<User> Users { get; set; } = ArraySegment<User>.Empty;
    }
}