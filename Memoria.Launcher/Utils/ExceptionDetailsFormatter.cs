#nullable enable

using System;
using System.ComponentModel;
using System.Text;

namespace Memoria.Launcher.Utils
{
    internal static class ExceptionDetailsFormatter
    {
        public static void AppendTo(StringBuilder message, Exception exception)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            Exception? current = exception;
            Int32 depth = 0;
            while (current != null)
            {
                if (depth > 0)
                    message.AppendLine().AppendLine().Append("Caused by: ");

                message.Append(current.GetType().Name).Append(": ").Append(current.Message);
                if (current is Win32Exception win32Exception)
                    message.AppendLine().Append("Native error code: ").Append(win32Exception.NativeErrorCode);

                current = current.InnerException;
                depth++;
            }
        }
    }
}
