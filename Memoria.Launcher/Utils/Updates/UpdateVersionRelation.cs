using System;

namespace Memoria.Launcher.Utils.Updates
{
    internal enum UpdateVersionRelation
    {
        Downgrade,
        Reinstall,
        Upgrade
    }

    internal static class UpdateVersionComparer
    {
        public static UpdateVersionRelation Compare(DateTime currentVersion, DateTime targetVersion)
        {
            Int32 comparison = DateTime.Compare(Normalize(currentVersion), Normalize(targetVersion));
            if (comparison > 0)
                return UpdateVersionRelation.Downgrade;
            if (comparison < 0)
                return UpdateVersionRelation.Upgrade;
            return UpdateVersionRelation.Reinstall;
        }

        private static DateTime Normalize(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value : value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
