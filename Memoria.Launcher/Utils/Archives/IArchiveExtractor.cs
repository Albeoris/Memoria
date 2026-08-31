#nullable enable

using System;
using System.Threading;

namespace Memoria.Launcher.Utils.Archives
{
    internal interface IArchiveExtractor
    {
        void Extract(
            String archivePath,
            String destinationPath,
            CancellationToken cancellationToken,
            Action<Int32>? progress = null);
    }
}
