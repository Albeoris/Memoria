using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;
using System.Threading;

namespace Memoria.Launcher
{
    public static class Extractor
    {
        public static void ExtractAllFileFromArchive(string archivePath, string extractTo, CancellationToken cancellationToken)
        {
            if (!File.Exists(archivePath))
            {
                return;
            }
            using (var archive = ArchiveFactory.Open(archivePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(extractTo, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    Directory.Delete(extractTo, true);
                }
            }
        }
    }
}
