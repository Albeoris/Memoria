using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Memoria.SteamFix
{
    internal class Program
    {
        const String FF9LauncherRegistryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FF9_Launcher.exe";

        public static void Main(String[] args)
        {
            try
            {
                if (args.Length == 0)
                    RollbackSteamOverlayFix();
                else
                    FixSteamOverlay(args[0]);
            }
            catch (Exception ex)
            {
                ExitWithError(ex.ToString());
            }
        }

        private static void RollbackSteamOverlayFix()
        {
            if (RemoveLauncherHook())
            {
                String launcherPath = Path.GetFullPath("FF9_Launcher.exe");
                String fixPath = Path.ChangeExtension(launcherPath, ".fix");

                if (!File.Exists(fixPath))
                {
                    Console.WriteLine($"Cannot find the new launcher to restore: {fixPath}. The original will be used instead.");
                }
                else
                {
                    KillRunningLaunchers();

                    try
                    {
                        File.Copy(fixPath, launcherPath, overwrite: true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to copy {fixPath} to {launcherPath}. Try copy it manually.", ex);
                    }
                }

                Console.WriteLine("We have successfully rolled back the fix. Steam will now launch FF9_Launcher.exe");
            }
            else
            {
                ExitWithError(@"No fix found.
                                         Run this application with the full path to the launcher file as a command line argument to reapply the patch.
                                         Example:
                                         Memoria.SteamFix.exe ""C:\Steam\steamapps\common\FINAL FANTASY IX\FF9_Launcher.exe""");
            }
        }

        private static void FixSteamOverlay(String launcherPath)
        {
            launcherPath = Path.GetFullPath(launcherPath);
            launcherPath = Path.ChangeExtension(launcherPath, ".exe");

            if (!launcherPath.Contains("FF9_Launcher"))
                throw new ArgumentException($"The launcher should be named \"FF9_Launcher\"");

            KillRunningLaunchers();

            String backupPath = Path.ChangeExtension(launcherPath, ".bak");
            String fixPath = Path.ChangeExtension(launcherPath, ".fix");

            ReplaceLauncher(launcherPath, backupPath, fixPath);
            SetLauncherHook(fixPath);
            Console.WriteLine("We have successfully fixed the overlay. Now when you start FF9_Launcher.exe, FF9_Launcher.fix will start instead.");
        }

        private static void KillRunningLaunchers()
        {
            try
            {
                String processName = Path.GetFileNameWithoutExtension("FF9_Launcher");
                Process[] processes = Process.GetProcesses().Where(p => p.ProcessName.StartsWith(processName)).ToArray();
                foreach (var process in processes)
                {
                    process.Kill();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to kill running launchers. This can lead to further errors.");
                Console.WriteLine(ex);
            }
        }

        private static void ReplaceLauncher(String launcherPath, String backupPath, String fixPath)
        {
            if (!File.Exists(launcherPath))
                throw new FileNotFoundException($"Cannot find the current launcher: {launcherPath}", launcherPath);

            if (!File.Exists(backupPath))
                throw new FileNotFoundException($"Cannot find a backup copy of the original launcher: {backupPath}", backupPath);

            try
            {
                File.Copy(launcherPath, fixPath, overwrite: true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to copy {launcherPath} to {fixPath}. Try running the launcher with administrator rights.", ex);
            }

            try
            {
                File.Copy(backupPath, launcherPath, overwrite: true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to copy {backupPath} to {launcherPath}. Try running the launcher with administrator rights.", ex);
            }
        }

        private static void SetLauncherHook(String fixPath)
        {
            try
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\" + FF9LauncherRegistryPath, "Debugger", fixPath, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to set launcher hook. Try running the launcher with administrator rights.", ex);
            }
        }

        private static Boolean RemoveLauncherHook()
        {
            try
            {
                using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
                {
                    using (var subKey = registryKey.OpenSubKey(FF9LauncherRegistryPath))
                    {
                        if (subKey == null)
                            return false;
                    }

                    registryKey.DeleteSubKey(FF9LauncherRegistryPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove launcher hook. Please delete the registry key manually: HKEY_LOCAL_MACHINE\\{FF9LauncherRegistryPath}", ex);
            }
        }

        private static void ExitWithError(String message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
