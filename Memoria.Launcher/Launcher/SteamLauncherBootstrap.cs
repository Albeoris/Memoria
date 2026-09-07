using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Memoria.Launcher
{
    internal static class SteamLauncherBootstrap
    {
        internal const UInt32 ApplicationId = 377840;

        public static Boolean RestartIfNecessary(String gameDirectory)
        {
            return RestartIfNecessary(gameDirectory, InvokeRestartAppIfNecessary);
        }

        internal static Boolean RestartIfNecessary(String gameDirectory, Func<String, UInt32, Boolean> restartAppIfNecessary)
        {
            if (String.IsNullOrEmpty(gameDirectory))
                throw new ArgumentException("The game directory is required.", nameof(gameDirectory));
            if (restartAppIfNecessary == null)
                throw new ArgumentNullException(nameof(restartAppIfNecessary));

            String steamApiPath = Path.Combine(gameDirectory, "x64", "steam_api64.dll");
            if (!File.Exists(steamApiPath))
                return false;

            String appIdPath = Path.Combine(gameDirectory, "steam_appid.txt");
            Byte[] appIdFileContents = Array.Empty<Byte>();
            Boolean appIdFileRemoved = false;
            Boolean restarted = false;

            if (File.Exists(appIdPath))
            {
                // SteamAPI_RestartAppIfNecessary always returns false while steam_appid.txt exists, even when the launcher was not started through Steam.
                // Temporarily remove the file created by the launcher so Steam can detect a manual launch; it is restored below if no restart occurs.
                String appIdText = File.ReadAllText(appIdPath).Trim();
                if (!UInt32.TryParse(appIdText, NumberStyles.None, CultureInfo.InvariantCulture, out UInt32 appId) || appId != ApplicationId)
                    return false;

                appIdFileContents = File.ReadAllBytes(appIdPath);
                File.Delete(appIdPath);
                appIdFileRemoved = true;
            }

            try
            {
                restarted = restartAppIfNecessary(steamApiPath, ApplicationId);
                return restarted;
            }
            finally
            {
                if (appIdFileRemoved && !restarted && !File.Exists(appIdPath))
                    File.WriteAllBytes(appIdPath, appIdFileContents);
            }
        }

        private static Boolean InvokeRestartAppIfNecessary(String steamApiPath, UInt32 appId)
        {
            IntPtr module = LoadLibrary(steamApiPath);
            if (module == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error(), $"Unable to load Steamworks API: {steamApiPath}");

            try
            {
                IntPtr address = GetProcAddress(module, "SteamAPI_RestartAppIfNecessary");
                if (address == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "SteamAPI_RestartAppIfNecessary is not available.");

                RestartAppIfNecessary restart = (RestartAppIfNecessary)Marshal.GetDelegateForFunctionPointer(address, typeof(RestartAppIfNecessary));
                return restart(appId);
            }
            finally
            {
                FreeLibrary(module);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private delegate Boolean RestartAppIfNecessary(UInt32 appId);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(String fileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr module, String procedureName);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean FreeLibrary(IntPtr module);
    }
}
