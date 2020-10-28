using System.Text.RegularExpressions;

namespace GitLab.Majordome.Logic
{
    internal static class StringExtensions
    {
        public static string EscapeMarkdown(this string s)
        {
            return Regex.Replace(s, @"([|\\*-.+!_])", @"\$1");
        }
    }
}