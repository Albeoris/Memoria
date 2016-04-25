using System;
using System.IO;

namespace Memoria
{
    public static class FileCommander
    {
        public static void PrepareFileDirectory(String filePath)
        {
            String directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath != null)
                Directory.CreateDirectory(directoryPath);
        }
    }
}
