using System;
using System.IO;

namespace Memoria.Patcher
{
    public sealed class GameLocationInfo
    {
        public readonly string RootDirectory;
        public readonly string ManagedPathX64;
        public readonly string ManagedPathX86;

        public const string LauncherName = @"FF9_Launcher.exe";
        private const string ManagedRelativePathX64 = @"x64\FF9_Data\Managed";
        private const string ManagedRelativePathX86 = @"x86\FF9_Data\Managed";

        public string LauncherPath => Path.Combine(RootDirectory, LauncherName);

        public GameLocationInfo(String rootDirectory)
        {
            RootDirectory = rootDirectory;
            ManagedPathX64 = Path.Combine(rootDirectory, ManagedRelativePathX64);
            ManagedPathX86 = Path.Combine(rootDirectory, ManagedRelativePathX86);
        }

        public void Validate()
        {
            Exceptions.CheckFileNotFoundException(LauncherPath);
        }
    }
}