#nullable enable

using System;
using System.Threading;

namespace Memoria.Launcher.Utils.Archives
{
    internal sealed class ArchiveExtractor : IArchiveExtractor
    {
        private readonly IArchiveExtractor _rarExtractor;
        private readonly IArchiveExtractor _otherFormatsExtractor;

        public ArchiveExtractor(String gameRootPath)
            : this(
                new SharpCompressRarExtractor(gameRootPath),
                new SevenZipExtractor(gameRootPath))
        {
        }

        internal ArchiveExtractor(IArchiveExtractor rarExtractor, IArchiveExtractor otherFormatsExtractor)
        {
            _rarExtractor = rarExtractor ?? throw new ArgumentNullException(nameof(rarExtractor));
            _otherFormatsExtractor = otherFormatsExtractor ?? throw new ArgumentNullException(nameof(otherFormatsExtractor));
        }

        public void Extract(
            String archivePath,
            String destinationPath,
            CancellationToken cancellationToken,
            Action<Int32>? progress = null)
        {
            IArchiveExtractor extractor = ArchiveExtractionBackendSelector.Select(archivePath) == ArchiveExtractionBackend.SharpCompress
                ? _rarExtractor
                : _otherFormatsExtractor;

            extractor.Extract(archivePath, destinationPath, cancellationToken, progress);
        }
    }
}
