using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer.Classes
{
    class RegistryValues
    {
        private static RegistryValues _instance;
        private static String _gameInstallPath;
        private RegistryValues() { }

        public static RegistryValues Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RegistryValues();
                }
                return _instance;
            }
        }


        public string? GetGameInstallPath
        {
            get
            {
                if (_gameInstallPath == null)
                {
                    string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 377840";
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
                    {
                        if (key != null)
                        {
                            _gameInstallPath = key.GetValue("InstallLocation") as string;
                        }
                    }
                }
                return _gameInstallPath;
            }
        }

        public void AddToUninstallList(string InstallPath)
        {
            string appName = "Memoria";
            string displayName = "Memoria Engine for FFIX";
            string displayVersion = "1.0.0";
            string publisher = "Memoria Team";
            string installLocation = InstallPath;
            string uninstallString = InstallPath+@"\setup.exe uninstall";
            string modifyPath = InstallPath + @"\setup.exe modify";
            string repairPath = InstallPath + @"\setup.exe repair";
            string iconPath = InstallPath + @"\setup.exe,0";
            string installDate = DateTime.Now.ToString("yyyyMMdd");

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                if (key != null)
                {
                    using (RegistryKey appKey = key.CreateSubKey(appName))
                    {
                        if (appKey != null)
                        {
                            appKey.SetValue("DisplayName", displayName);
                            appKey.SetValue("DisplayVersion", displayVersion);
                            appKey.SetValue("Publisher", publisher);
                            appKey.SetValue("InstallLocation", installLocation);
                            appKey.SetValue("UninstallString", uninstallString);
                            appKey.SetValue("InstallDate", installDate);
                            appKey.SetValue("ModifyPath", modifyPath);
                            appKey.SetValue("RepairPath", repairPath);
                            appKey.SetValue("DisplayIcon", iconPath);
                        }
                    }
                }
            }
        }

        public void RemoveFromUninstallList()
        {
            string appName = "Memoria";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                if (key != null)
                {
                    key.DeleteSubKeyTree(appName);
                }
            }
        }
    }
}
