#nullable disable

using System;
using System.Collections.Generic;

namespace Memoria.Launcher.Utils.Updates
{
    internal enum ReleaseNotesBlockKind
    {
        Heading,
        Paragraph,
        Bullet,
        Link
    }

    internal sealed class ReleaseNotesBlock
    {
        public ReleaseNotesBlock(ReleaseNotesBlockKind kind, String text, Uri link = null)
        {
            Kind = kind;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Link = link;
        }

        public ReleaseNotesBlockKind Kind { get; }
        public String Text { get; }
        public Uri Link { get; }
    }

    internal sealed class ReleaseNotesDocument
    {
        public ReleaseNotesDocument(UpdateBuild build, String title, String tag, Uri releasePage, IReadOnlyList<ReleaseNotesBlock> blocks)
        {
            Build = build ?? throw new ArgumentNullException(nameof(build));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            ReleasePage = releasePage ?? throw new ArgumentNullException(nameof(releasePage));
            Blocks = blocks ?? throw new ArgumentNullException(nameof(blocks));
        }

        public UpdateBuild Build { get; }
        public String Title { get; }
        public String Tag { get; }
        public Uri ReleasePage { get; }
        public IReadOnlyList<ReleaseNotesBlock> Blocks { get; }
    }
}
