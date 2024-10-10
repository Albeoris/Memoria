using Microsoft.Win32;
using System;

namespace Memoria.Patcher
{
    public sealed class GameLocationSteamRegistryProvider
    {
        public const String SteamRegistyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 377840";
        public const String SteamGamePathTag = @"InstallLocation";

        public static GameLocationInfo TryLoad()
        {
            return TryLoadLocation(RegistryView.Registry64) ?? TryLoadLocation(RegistryView.Registry32);
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

                //var bak = new FileInfo("FF9_Launcher.bak");
                //var exe = new FileInfo("FF9_Launcher.exe");
                //
                //if (bak.Exists && exe.Exists && bak.Length != exe.Length)
                //    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
