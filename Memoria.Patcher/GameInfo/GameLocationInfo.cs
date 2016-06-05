using System;
using System.IO;

namespace Memoria.Patcher
{
    public sealed class GameLocationInfo
    {
        public readonly String RootDirectory;
        public readonly String ManagedPathX64;
        public readonly String ManagedPathX86;
        public readonly String StreamingAssetsPath;

        public const String LauncherName = @"FF9_Launcher.exe";
        private const String ManagedRelativePathX64 = @"x64\FF9_Data\Managed";
        private const String ManagedRelativePathX86 = @"x86\FF9_Data\Managed";
        private const String StreamingAssetsRelativePath = @"StreamingAssets";

        public String LauncherPath => Path.Combine(RootDirectory, LauncherName);

        public GameLocationInfo(String rootDirectory)
        {
            RootDirectory = rootDirectory;
            ManagedPathX64 = Path.Combine(rootDirectory, ManagedRelativePathX64);
            ManagedPathX86 = Path.Combine(rootDirectory, ManagedRelativePathX86);
            StreamingAssetsPath = Path.Combine(rootDirectory, StreamingAssetsRelativePath);
        }

        public void Validate()
        {
            Exceptions.CheckFileNotFoundException(LauncherPath);
        }
    }
}