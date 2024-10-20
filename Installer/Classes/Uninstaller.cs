using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer.Classes
{
    internal class Uninstaller
    {
        private static string GetSteamPath()
        {
            string steamPath = null;

            // Check for 64-bit registry key
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam"))
            {
                if (key != null)
                {
                    steamPath = key.GetValue("InstallPath") as string;
                }
            }

            // Check for 32-bit registry key if 64-bit key is not found
            if (steamPath == null)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam"))
                {
                    if (key != null)
                    {
                        steamPath = key.GetValue("InstallPath") as string;
                    }
                }
            }

            return steamPath;
        }

        private static void CleanGameFiles()
        {
            string steamPath = GetSteamPath();
            string arguments = "-validate 377840";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = steamPath,
                Arguments = arguments,
                UseShellExecute = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        // don't actually run it's just here so i can keep track of what needs to run in this senario
        private static void Run()
        {
            CleanGameFiles();
            RegistryValues.Instance.RemoveFromUninstallList();
        }
    }
}
