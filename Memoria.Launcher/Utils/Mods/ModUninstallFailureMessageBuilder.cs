#nullable enable

using System;
using System.Text;
using Memoria.Launcher.Utils;

namespace Memoria.Launcher.Utils.Mods
{
    internal static class ModUninstallFailureMessageBuilder
    {
        public static String Build(String modName, String installationPath, Exception exception)
        {
            ValidateRequiredText(modName, nameof(modName));
            ValidateRequiredText(installationPath, nameof(installationPath));

            StringBuilder message = new StringBuilder("The mod could not be removed.")
                .AppendLine()
                .Append("Mod: ").Append(modName)
                .AppendLine()
                .Append("Installation path: ").Append(installationPath)
                .AppendLine()
                .AppendLine()
                .AppendLine("Failure details:");
            ExceptionDetailsFormatter.AppendTo(message, exception);
            return message.ToString();
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
