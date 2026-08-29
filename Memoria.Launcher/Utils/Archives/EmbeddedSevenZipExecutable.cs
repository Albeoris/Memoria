using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security;

namespace Memoria.Launcher.Utils.Archives
{
    internal sealed class EmbeddedSevenZipExecutable
    {
        private const String ResourceName = "7za.exe";
        private const String ExecutableName = "7za.exe";

        private readonly Assembly _resourceAssembly;

        public EmbeddedSevenZipExecutable(Assembly resourceAssembly)
        {
            _resourceAssembly = resourceAssembly ?? throw new ArgumentNullException(nameof(resourceAssembly));
        }

        public String GetPath()
        {
            try
            {
                Byte[] executable = ReadResource();
                String fingerprint = CalculateFingerprint(executable);
                String localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (String.IsNullOrWhiteSpace(localApplicationData))
                    localApplicationData = Path.GetTempPath();

                String toolsDirectory = Path.Combine(
                    localApplicationData,
                    "Memoria",
                    "Tools",
                    "7-Zip",
                    fingerprint);
                String executablePath = Path.Combine(toolsDirectory, ExecutableName);

                Directory.CreateDirectory(toolsDirectory);
                if (IsCurrentExecutable(executablePath, executable))
                    return executablePath;

                WriteAtomically(executablePath, executable);
                return executablePath;
            }
            catch (ArchiveExtractionException)
            {
                throw;
            }
            catch (Exception exception) when (exception is UnauthorizedAccessException || exception is SecurityException || exception is IOException)
            {
                throw new ArchiveExtractionException(
                    "Memoria could not prepare its bundled 7-Zip executable in your local application-data folder. " +
                    "Check free disk space and allow Memoria Launcher and 7za.exe in your antivirus or Windows Controlled Folder Access settings.",
                    exception);
            }
        }

        private Byte[] ReadResource()
        {
            using Stream input = _resourceAssembly.GetManifestResourceStream(ResourceName);
            if (input == null)
            {
                throw new ArchiveExtractionException(
                    "The launcher installation is incomplete because its embedded 7-Zip executable is missing. " +
                    "Reinstall or update Memoria Launcher, then try again.");
            }

            using MemoryStream output = new MemoryStream();
            input.CopyTo(output);
            return output.ToArray();
        }

        private static String CalculateFingerprint(Byte[] content)
        {
            using SHA256 sha256 = SHA256.Create();
            Byte[] hash = sha256.ComputeHash(content);
            return BitConverter.ToString(hash, 0, 12).Replace("-", String.Empty);
        }

        private static Boolean IsCurrentExecutable(String executablePath, Byte[] expectedContent)
        {
            if (!File.Exists(executablePath))
                return false;

            FileInfo file = new FileInfo(executablePath);
            if (file.Length != expectedContent.LongLength)
                return false;

            using FileStream input = new FileStream(executablePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Byte[] buffer = new Byte[32 * 1024];
            Int32 expectedOffset = 0;
            while (expectedOffset < expectedContent.Length)
            {
                Int32 read = input.Read(buffer, 0, Math.Min(buffer.Length, expectedContent.Length - expectedOffset));
                if (read == 0)
                    return false;

                for (Int32 index = 0; index < read; index++)
                {
                    if (buffer[index] != expectedContent[expectedOffset + index])
                        return false;
                }
                expectedOffset += read;
            }

            return input.ReadByte() == -1;
        }

        private static void WriteAtomically(String executablePath, Byte[] content)
        {
            String temporaryPath = executablePath + $".{Guid.NewGuid():N}.tmp";
            try
            {
                using (FileStream output = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    output.Write(content, 0, content.Length);
                    output.Flush(flushToDisk: true);
                }

                try
                {
                    if (File.Exists(executablePath))
                        File.Replace(temporaryPath, executablePath, destinationBackupFileName: null);
                    else
                        File.Move(temporaryPath, executablePath);
                }
                catch (IOException) when (IsCurrentExecutable(executablePath, content))
                {
                    // Another launcher instance prepared the same executable first.
                }
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }
    }
}
