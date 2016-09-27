using System;
using Microsoft.Win32;

namespace Memoria.Patcher
{
    public sealed class GameLocationSteamRegistryProvider
    {
        public const String SteamRegistyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 377840";
        public const String SteamGamePathTag = @"InstallLocation";

        public static GameLocationInfo TryLoad()
        {
            return TryLoadLocation(RegistryView.Registry64)
                   ?? TryLoadLocation(RegistryView.Registry32);
        }

        private static GameLocationInfo TryLoadLocation(RegistryView view)
        {
            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                return TryLoadLocation(localMachine);
        }

        private static GameLocationInfo TryLoadLocation(RegistryKey localMachine)
        {
            using (RegistryKey registryKey = localMachine.OpenSubKey(SteamRegistyPath))
            {
                if (registryKey == null)
                    return null;

                GameLocationInfo result = new GameLocationInfo((String)registryKey.GetValue(SteamGamePathTag));
                result.Validate();

                return result;
            }
        }
    }
}