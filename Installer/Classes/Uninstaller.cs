using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Installer.Classes
{
    internal class Uninstaller
    {
        private static bool isX64 = false;
        private static string GetSteamPath()
        {
            string steamPath = null;

            // Check for 64-bit registry key
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam"))
            {
                if (key != null)
                {
                    isX64 = true;
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

        public static async void CleanGameFiles(String gamePath)
        {
            if (File.Exists(gamePath + @"\FF9_Launcher.bak"))
            {
                File.Delete(gamePath + @"\FF9_Launcher.exe");
                File.Move(gamePath + @"\FF9_Launcher.bak", gamePath + @"\FF9_Launcher.exe");
            }
        }


    }
}
