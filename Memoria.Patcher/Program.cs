using System;
using System.IO;
using System.Reflection;
using Memoria.Prime;

namespace Memoria.Patcher
{
    static class Program
    {
        static void Main(String[] args)
        {
            try
            {
                GameLocationInfo gameLocation = GetGameLocation(args);
                if (gameLocation == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("{0}.exe <gamePath>", Assembly.GetExecutingAssembly().GetName().Name);
                    Console.WriteLine("Press enter to exit...");
                    Console.ReadLine();
                    Environment.Exit(1);
                }

                ReplaceLauncher(gameLocation);
                ReplaceDebugger(gameLocation);
                CopyExternalFiles(gameLocation.StreamingAssetsPath);
                Patch(gameLocation.ManagedPathX64);
                Patch(gameLocation.ManagedPathX86);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error has occurred. See [{Log.LogFileName}] for details.");
                Console.WriteLine(ex);

                Log.Error(ex, "Unexpected error.");
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void ReplaceLauncher(GameLocationInfo gameLocation)
        {
            String sourceDirectory = Path.GetFullPath("Launcher");
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException("Launcher's directory was not found: " + sourceDirectory);

            String backupPath = Path.ChangeExtension(gameLocation.LauncherPath, ".bak");
            if (!File.Exists(backupPath))
            {
                Console.WriteLine("Back up a launcher.");
                File.Copy(gameLocation.LauncherPath, backupPath);
            }

            Console.WriteLine("Copy a launcher...");
            foreach (String sourcePath in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.TopDirectoryOnly))
            {
                if (!sourcePath.StartsWith(sourceDirectory))
                    throw new InvalidDataException(sourcePath);

                String relativePath = sourcePath.Substring(sourceDirectory.Length);
                relativePath = relativePath.Replace("Memoria.Launcher", "FF9_Launcher");
                String targetPath = gameLocation.RootDirectory + relativePath;

                File.Copy(sourcePath, targetPath, true);
                Console.WriteLine("Copied: " + targetPath.Substring(gameLocation.RootDirectory.Length));

            }
            Console.WriteLine("The launcher was copied!");
        }

        private static void ReplaceDebugger(GameLocationInfo gameLocation)
        {
            String sourceDirectory = Path.GetFullPath("Debugger");
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException("Debugger's directory was not found: " + sourceDirectory);

            Console.WriteLine("Copy a debugger...");

            String targetDirectory = Path.Combine(gameLocation.RootDirectory, "Debugger");
            Directory.CreateDirectory(targetDirectory);

            if (FsHelper.IsSamePaths(sourceDirectory, targetDirectory))
            {
                Console.WriteLine("Copying skipped because source and target folders have a same path.");
                return;
            }

            foreach (String sourcePath in Directory.EnumerateFileSystemEntries(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                if (!sourcePath.StartsWith(sourceDirectory))
                    throw new InvalidDataException(sourcePath);

                String relativePath = sourcePath.Substring(sourceDirectory.Length);
                String targetPath = targetDirectory + relativePath;

                if ((File.GetAttributes(sourcePath) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Directory.CreateDirectory(targetPath);
                }
                else
                {
                    File.Copy(sourcePath, targetPath, true);
                    Console.WriteLine("Copied: " + targetPath.Substring(targetDirectory.Length));
                }

            }
            Console.WriteLine("The debugger was copied!");
        }

        private static void CopyExternalFiles(String targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
                throw new DirectoryNotFoundException("StreamingAssets directory does not exist: " + targetDirectory);

            CopyData(targetDirectory);
            CopyScripts(targetDirectory);
            CopyShaders(targetDirectory);
        }

        private static void CopyData(String targetDirectory)
        {
            String sourceDirectory = Path.GetFullPath("Data");
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException("Data files was not found: " + sourceDirectory);

            targetDirectory = Path.Combine(targetDirectory, "Data");

            Console.WriteLine("Copy data files...");
            CopyFiles(targetDirectory, sourceDirectory, "*.csv");
            Console.WriteLine("Data files was copied!");
        }

        private static void CopyScripts(String targetDirectory)
        {
            String sourceDirectory = Path.GetFullPath("Scripts");
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException("Script files was not found: " + sourceDirectory);

            targetDirectory = Path.Combine(targetDirectory, "Scripts");

            Console.WriteLine("Copy script files...");
            CopyFiles(targetDirectory, sourceDirectory, "*.*");
            Console.WriteLine("Script files was copied!");
        }

        private static void CopyShaders(String targetDirectory)
        {
            String sourceDirectory = Path.GetFullPath("Shaders");
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException("Shaders files was not found: " + sourceDirectory);

            targetDirectory = Path.Combine(targetDirectory, "Shaders");

            Console.WriteLine("Copy shader files...");
            CopyFiles(targetDirectory, sourceDirectory, "*.*");
            Console.WriteLine("Shaders files was copied!");
        }

        private static void CopyFiles(String targetDirectory, String sourceDirectory, String extensions)
        {
            if (FsHelper.IsSamePaths(sourceDirectory, targetDirectory))
            {
                Console.WriteLine("Copying skipped because source and target folders have a same path.");
                return;
            }

            foreach (String sourceFile in Directory.EnumerateFiles(sourceDirectory, extensions, SearchOption.AllDirectories))
            {
                DateTime sourceFileTime = File.GetLastWriteTimeUtc(sourceFile);
                String targetFile = targetDirectory + sourceFile.Substring(sourceDirectory.Length);
                if (File.Exists(targetFile) && File.GetLastWriteTimeUtc(targetFile) == sourceFileTime)
                    continue;

                String directoryName = Path.GetDirectoryName(targetFile);
                Directory.CreateDirectory(directoryName);

                File.Copy(sourceFile, targetFile, true);
                File.SetCreationTimeUtc(targetFile, sourceFileTime);
                Console.WriteLine("Copied: " + targetFile.Substring(targetDirectory.Length));
            }
        }

        private static void Patch(String directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Console.WriteLine("Directory does not exist: {0}", directory);
                    return;
                }

                Console.WriteLine("Patching...");

                String assemblyPath = Path.Combine(directory, "Assembly-CSharp.dll");
                String backupPath = Path.Combine(directory, "Assembly-CSharp.bak");

                if (!File.Exists(backupPath))
                    File.Copy(assemblyPath, backupPath);

                String primeDllPath = Path.Combine(directory, "Memoria.Prime.dll");
                File.Copy("Memoria.Prime.dll", primeDllPath, true);
                File.Copy("Memoria.Prime.dll.mdb", primeDllPath + ".mdb", true);

                String unityUiDllPath = Path.Combine(directory, "UnityEngine.UI.dll");
                File.Copy("UnityEngine.UI.dll", unityUiDllPath, true);
                File.Copy("UnityEngine.UI.dll.mdb", unityUiDllPath + ".mdb", true);

                File.Copy("Assembly-CSharp.dll", assemblyPath, true);
                File.Copy("Assembly-CSharp.dll.mdb", assemblyPath + ".mdb", true);

                Console.WriteLine("Success!");
            }
            catch (Exception ex)
            {
                String message = $"Failed to patch assembly from a directory [{directory}]";
                Console.WriteLine(message);
                Log.Error(ex, message);
            }
        }

        private static GameLocationInfo GetGameLocation(String[] args)
        {
            try
            {
                GameLocationInfo result;
                if (args.IsNullOrEmpty())
                {
                    if (File.Exists(GameLocationInfo.LauncherName))
                    {
                        result = new GameLocationInfo(Environment.CurrentDirectory);
                        result.Validate();
                    }
                    else
                    {
                        result = GameLocationSteamRegistryProvider.TryLoad();
                    }
                }
                else
                {
                    result = new GameLocationInfo(args[0]);
                    result.Validate();
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get a game location.");
                Console.WriteLine($"Failed to get a game location. See [{Log.LogFileName}] for details.");
                return null;
            }
        }
    }
}