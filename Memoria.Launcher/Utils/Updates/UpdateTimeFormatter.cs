using System;
using System.Globalization;

namespace Memoria.Launcher.Utils.Updates
{
    internal static class UpdateTimeFormatter
    {
        private const String LocalFormat = "yyyy.MM.dd HH:mm";
        private const String UtcDateTimeFormat = "yyyy.MM.dd HH:mm 'UTC'";
        private const String UtcTimeFormat = "HH:mm 'UTC'";

        public static String FormatLocal(DateTime value, TimeZoneInfo localTimeZone)
        {
            if (localTimeZone == null)
                throw new ArgumentNullException(nameof(localTimeZone));

            DateTime local = TimeZoneInfo.ConvertTimeFromUtc(NormalizeAsUtc(value), localTimeZone);
            return local.ToString(LocalFormat, CultureInfo.CurrentCulture);
        }

        public static String FormatWithUtc(DateTime value, TimeZoneInfo localTimeZone)
        {
            if (localTimeZone == null)
                throw new ArgumentNullException(nameof(localTimeZone));

            DateTime utc = NormalizeAsUtc(value);
            DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utc, localTimeZone);
            String utcFormat = local.Date == utc.Date ? UtcTimeFormat : UtcDateTimeFormat;
            return $"{local.ToString(LocalFormat, CultureInfo.CurrentCulture)} ({utc.ToString(utcFormat, CultureInfo.InvariantCulture)})";
        }

        private static DateTime NormalizeAsUtc(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value : value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
