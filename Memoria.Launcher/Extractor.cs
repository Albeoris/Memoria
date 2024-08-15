using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Launcher
{
    public class Extractor
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
