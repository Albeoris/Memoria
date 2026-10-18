#nullable disable

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Memoria.Launcher.Utils.Updates
{
    internal static class GitHubReleaseDescriptionParser
    {
        private static readonly Regex HeadingPattern = new Regex(@"^#{1,6}\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex BulletPattern = new Regex(@"^\s*(?:[-*+]\s+|\d+\.\s+)(.+)$", RegexOptions.Compiled);
        private static readonly Regex LinkPattern = new Regex(@"^\[([^\]]+)\]\((https?://[^)]+)\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ImagePattern = new Regex(@"^!\[[^\]]*\]\([^)]+\)$", RegexOptions.Compiled);
        private static readonly Regex InlineLinkPattern = new Regex(@"\[([^\]]+)\]\([^)]+\)", RegexOptions.Compiled);
        private static readonly Regex HtmlPattern = new Regex(@"<[^>]+>", RegexOptions.Compiled);

        public static IReadOnlyList<ReleaseNotesBlock> Parse(String markdown)
        {
            List<ReleaseNotesBlock> result = new List<ReleaseNotesBlock>();
            if (String.IsNullOrWhiteSpace(markdown))
                return result;

            String[] lines = markdown.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            foreach (String sourceLine in lines)
            {
                String line = sourceLine.Trim();
                if (line.Length == 0 || line == "---" || ImagePattern.IsMatch(line))
                    continue;

                Match heading = HeadingPattern.Match(line);
                if (heading.Success)
                {
                    Add(result, ReleaseNotesBlockKind.Heading, heading.Groups[1].Value);
                    continue;
                }

                Match bullet = BulletPattern.Match(line);
                if (bullet.Success)
                {
                    Add(result, ReleaseNotesBlockKind.Bullet, bullet.Groups[1].Value);
                    continue;
                }

                Match link = LinkPattern.Match(line);
                if (link.Success && Uri.TryCreate(link.Groups[2].Value, UriKind.Absolute, out Uri linkUri))
                {
                    result.Add(new ReleaseNotesBlock(ReleaseNotesBlockKind.Link, CleanInlineMarkup(link.Groups[1].Value), linkUri));
                    continue;
                }

                Add(result, ReleaseNotesBlockKind.Paragraph, line);
            }
            return result;
        }

        private static void Add(List<ReleaseNotesBlock> result, ReleaseNotesBlockKind kind, String markdown)
        {
            String text = CleanInlineMarkup(markdown);
            if (text.Length > 0)
                result.Add(new ReleaseNotesBlock(kind, text));
        }

        private static String CleanInlineMarkup(String value)
        {
            String text = InlineLinkPattern.Replace(value, "$1");
            text = HtmlPattern.Replace(text, String.Empty);
            text = text.Replace("**", String.Empty).Replace("__", String.Empty).Replace("`", String.Empty);
            return WebUtility.HtmlDecode(text).Trim();
        }
    }
}
