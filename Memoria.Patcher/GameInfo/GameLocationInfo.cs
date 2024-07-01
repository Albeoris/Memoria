using System;
using System.Diagnostics;
using System.IO;

namespace Memoria.Patcher
{
    public sealed class GameLocationInfo
    {
        public readonly String RootDirectory;
        public readonly String ManagedPathX64;
        public readonly String ManagedPathX86;
        public readonly String StreamingAssetsPath;
        public readonly String EmbeddedAssetPath;

        public const String LauncherName = @"FF9_Launcher.exe";
        private const String ManagedRelativePathX64 = @"x64\FF9_Data\Managed";
        private const String ManagedRelativePathX86 = @"x86\FF9_Data\Managed";
        private const String StreamingAssetsRelativePath = @"StreamingAssets";
        private const String EmbeddedAssetRelativePath = @"FF9_Data";

        public String LauncherPath => Path.Combine(RootDirectory, LauncherName);

        public GameLocationInfo(String rootDirectory)
        {
            RootDirectory = rootDirectory;
            ManagedPathX64 = Path.Combine(rootDirectory, ManagedRelativePathX64);
            ManagedPathX86 = Path.Combine(rootDirectory, ManagedRelativePathX86);
            StreamingAssetsPath = Path.Combine(rootDirectory, StreamingAssetsRelativePath);
            EmbeddedAssetPath = Path.Combine(rootDirectory, EmbeddedAssetRelativePath);
        }

        public void Validate()
        {
            if (!File.Exists(LauncherPath))
                throw new FileNotFoundException(LauncherPath, LauncherPath);
        }

        public Boolean IsValid()
        {
            return File.Exists(LauncherPath);
        }

        public Boolean FixSteamOverlay(Boolean fix)
        {
            String fixerPath = Path.Combine(RootDirectory, "Memoria.SteamFix.exe");
            if (!File.Exists(fixerPath))
                return false;

            Console.WriteLine(fix ? "Re-enabling Steam overlay fix." : "Disabling Steam overlay fix.");
            Process process;
            if (fix)
                process = Process.Start(new ProcessStartInfo(fixerPath, @$" ""{LauncherPath}"" ") { Verb = "runas" });
            else
                process = Process.Start(new ProcessStartInfo(fixerPath) { Verb = "runas" });
            process.WaitForExit();
            return true;
        }
    }
}
