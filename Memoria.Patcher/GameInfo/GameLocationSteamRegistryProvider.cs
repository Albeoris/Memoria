using Microsoft.Win32;
using System;

namespace Memoria.Patcher
{
    public sealed class GameLocationSteamRegistryProvider
    {
        public const String SteamRegistyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 377840";
        public const String SteamGamePathTag = @"InstallLocation";
        public const String GogRegistryPath = @"SOFTWARE\GOG.com\Games\1375008492";
        public const String GogGamePathTag = @"path";

        public static GameLocationInfo TryLoad()
        {
            return TryLoadLocation(RegistryView.Registry64, SteamRegistyPath, SteamGamePathTag)
                   ?? TryLoadLocation(RegistryView.Registry32, SteamRegistyPath, SteamGamePathTag)
                   ?? TryLoadLocation(RegistryView.Registry64, GogRegistryPath, GogGamePathTag)
                   ?? TryLoadLocation(RegistryView.Registry32, GogRegistryPath, GogGamePathTag);
        }

        private static GameLocationInfo TryLoadLocation(RegistryView view, String registryPath, String gamePathTag)
        {
            using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                return TryLoadLocation(localMachine, registryPath, gamePathTag);
        }

        private static GameLocationInfo TryLoadLocation(RegistryKey localMachine, String registryPath, String gamePathTag)
        {
            using (RegistryKey registryKey = localMachine.OpenSubKey(registryPath))
            {
                if (registryKey == null)
                    return null;

                String gamePath = registryKey.GetValue(gamePathTag) as String;
                if (String.IsNullOrWhiteSpace(gamePath))
                    return null;

                GameLocationInfo result = new GameLocationInfo(gamePath);
                result.Validate();

                return result;
            }
        }

        public static Boolean IsSteamOverlayFixed()
        {
            try
            {
                using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
                {
                    using (var subKey = registryKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FF9_Launcher.exe"))
                    {
                        if (subKey?.GetValue("Debugger") == null)
                            return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
