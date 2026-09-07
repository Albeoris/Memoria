using Memoria.Launcher.Utils.Updates;
using Xunit;

namespace Memoria.Launcher.Tests.Updates;

public sealed class UpdateTimeFormatterTests
{
    private static readonly TimeZoneInfo UtcPlusTwo = TimeZoneInfo.CreateCustomTimeZone("UTC+02:00", TimeSpan.FromHours(2), "UTC+02:00", "UTC+02:00");

    [Fact]
    public void Same_calendar_day_uses_short_utc_time()
    {
        DateTime value = new DateTime(2026, 9, 7, 14, 26, 0, DateTimeKind.Utc);

        String result = UpdateTimeFormatter.FormatWithUtc(value, UtcPlusTwo);

        Assert.Equal("2026.09.07 16:26 (14:26 UTC)", result);
    }

    [Fact]
    public void Different_calendar_day_uses_full_utc_date_and_time()
    {
        DateTime value = new DateTime(2026, 9, 7, 23, 26, 0, DateTimeKind.Utc);

        String result = UpdateTimeFormatter.FormatWithUtc(value, UtcPlusTwo);

        Assert.Equal("2026.09.08 01:26 (2026.09.07 23:26 UTC)", result);
    }

    [Fact]
    public void Button_time_contains_only_local_date_and_time()
    {
        DateTime value = new DateTime(2026, 9, 7, 14, 26, 0, DateTimeKind.Utc);

        String result = UpdateTimeFormatter.FormatLocal(value, UtcPlusTwo);

        Assert.Equal("2026.09.07 16:26", result);
    }
}
