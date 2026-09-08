using System;
using System.Globalization;

namespace Memoria.Launcher.Utils.Updates
{
    internal static class InstalledBuildVersion
    {
        public static Boolean Matches(String storedValue, DateTime installedVersion)
        {
            return DateTime.TryParseExact(storedValue, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime storedVersion) && Normalize(storedVersion) == Normalize(installedVersion);
        }

        public static String Serialize(DateTime installedVersion)
        {
            return Normalize(installedVersion).ToString("O", CultureInfo.InvariantCulture);
        }

        private static DateTime Normalize(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value : value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
