#nullable enable

using System;
using System.Text;
using Memoria.Launcher.Utils;

namespace Memoria.Launcher.Utils.Archives
{
    internal static class ArchiveFailureMessageBuilder
    {
        public static String Build(String archivePath, Exception exception)
        {
            ValidateRequiredText(archivePath, nameof(archivePath));
            StringBuilder message = CreateHeader(archivePath);
            AppendFailureDetails(message, exception);
            return message.ToString();
        }

        public static String BuildForMod(String modName, String archivePath, Exception exception)
        {
            ValidateRequiredText(modName, nameof(modName));
            ValidateRequiredText(archivePath, nameof(archivePath));

            StringBuilder message = new StringBuilder("The mod archive could not be installed.")
                .AppendLine()
                .Append("Mod: ").Append(modName)
                .AppendLine()
                .Append("Archive: ").Append(archivePath);
            AppendFailureDetails(message, exception);
            return message.ToString();
        }

        public static String BuildCleanupFailureForMod(String modName, String archivePath, Exception exception)
        {
            ValidateRequiredText(modName, nameof(modName));
            ValidateRequiredText(archivePath, nameof(archivePath));

            StringBuilder message = new StringBuilder("The mod was installed, but its downloaded archive could not be deleted.")
                .AppendLine()
                .Append("Mod: ").Append(modName)
                .AppendLine()
                .Append("Archive: ").Append(archivePath);
            AppendFailureDetails(message, exception);
            return message.ToString();
        }

        private static StringBuilder CreateHeader(String archivePath) =>
            new StringBuilder("The mod archive could not be installed.")
                .AppendLine()
                .Append("Archive: ")
                .Append(archivePath);

        private static void AppendFailureDetails(StringBuilder message, Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            message.AppendLine().AppendLine().AppendLine("Failure details:");
            ExceptionDetailsFormatter.AppendTo(message, exception);
        }

        private static void ValidateRequiredText(String value, String parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("The value cannot be empty or whitespace.", parameterName);
        }
    }
}
