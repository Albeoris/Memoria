using System;
using System.Text;

namespace Memoria.Launcher.Utils.Archives
{
    internal static class SevenZipFailure
    {
        public static ArchiveExtractionException Create(SevenZipExitCode exitCode, String archivePath, String destinationPath, String standardError, String standardOutput)
        {
            if (String.IsNullOrWhiteSpace(archivePath))
                throw new ArgumentException("The archive path cannot be empty or whitespace.", nameof(archivePath));
            if (String.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("The destination path cannot be empty or whitespace.", nameof(destinationPath));

            StringBuilder message = new StringBuilder()
                .Append("7-Zip could not extract the archive.")
                .AppendLine()
                .Append("Exit code: ").Append((Int32)exitCode).Append(" (").Append(exitCode).Append(')')
                .AppendLine()
                .Append("Archive: ").Append(archivePath)
                .AppendLine()
                .Append("Destination: ").Append(destinationPath)
                .AppendLine()
                .Append("Recommendation: ").Append(GetRecommendation(exitCode));

            AppendProcessOutput(message, "Standard error", standardError);
            AppendProcessOutput(message, "Standard output", standardOutput);
            return new ArchiveExtractionException(message.ToString());
        }

        private static String GetRecommendation(SevenZipExitCode exitCode)
        {
            return exitCode switch
            {
                SevenZipExitCode.Warning => "The archive was only partially extracted. Download it again and verify that enough disk space is available.",
                SevenZipExitCode.FatalError => "The archive may be incomplete or corrupt. Download it again; if the problem persists, contact the mod author.",
                SevenZipExitCode.CommandLineError => "The bundled 7-Zip installation may be damaged. Reinstall or update Memoria Launcher.",
                SevenZipExitCode.NotEnoughMemory => "Close other applications or increase the Windows paging file, then try again.",
                SevenZipExitCode.Cancelled => "Extraction was stopped. Start the installation again when ready.",
                _ => "Check the archive, available disk space, antivirus settings, and write permission for the destination, then try again."
            };
        }

        private static void AppendProcessOutput(StringBuilder message, String title, String output)
        {
            if (String.IsNullOrWhiteSpace(output))
                return;

            message.AppendLine().AppendLine().Append(title).AppendLine(":").Append(output.Trim());
        }
    }
}
