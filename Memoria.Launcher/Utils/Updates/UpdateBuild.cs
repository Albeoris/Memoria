using System;

namespace Memoria.Launcher.Utils.Updates
{
    internal enum UpdateBuildKind
    {
        Stable,
        Canary
    }

    internal sealed class UpdateBuild
    {
        public UpdateBuild(UpdateBuildKind kind, Uri source)
        {
            Kind = kind;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            if (!source.IsAbsoluteUri)
                throw new ArgumentException("The update URI must be absolute.", nameof(source));
            ManifestSource = new Uri(source, "manifest.json");
        }

        public UpdateBuildKind Kind { get; }
        public Uri Source { get; }
        public Uri ManifestSource { get; }
        public String Name => Kind.ToString();
    }
}
