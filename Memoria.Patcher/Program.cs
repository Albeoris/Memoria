using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Memoria.Patcher
{
    static class Program
    {
        static Double ProgressPercent;
        static void Main(String[] args)
        {
            try
            {
                if (args.Length > 1 && args[0] == "-update")
                {
                    String launcherProcessPath = args[1];
                    String launcherProcessDirectory = Path.GetDirectoryName(launcherProcessPath);

                    if (args.Length > 2)
                    {
                        Int32 launcherProcessId = Int32.Parse(args[2]);
                        try
                        {
                            Process process = Process.GetProcessById(launcherProcessId);
                            process.Kill();
                            process.WaitForExit();
                        }
                        catch
                        {
                        }

                        Run(new[] { launcherProcessDirectory });

                        String arguments = $"-update \"{launcherProcessPath}\"";
                        foreach (String patcher in args.Skip(3))
                        {
                            Process process = Process.Start(patcher, arguments);
                            process?.WaitForExit();
                        }

                        // Send a command "FF9_Launcher.exe" even when the Steam overlay fix redirects to "FF9_Launcher.fix"
                        launcherProcessPath = Path.ChangeExtension(launcherProcessPath, ".exe");
                        Process.Start(launcherProcessPath);
                    }
                    else
                    {
                        Run(new[] { launcherProcessDirectory });
                    }

                    Environment.Exit(0);
                }
                else
                {
                    Run(args);
                    if (ProgressPercent == 100)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("V ");
                        Console.ResetColor();
                        Console.WriteLine(Lang.Message.Done.Success);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("X ");
                        Console.ResetColor();
                        Console.WriteLine($"The patching could only complete {ProgressPercent}%");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unexpected error has occurred.");
                Console.ResetColor();
                Console.WriteLine("---------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("---------------------------");
            }

            
            if (ProgressPercent == 100)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("V ");
                Console.ResetColor();
                //Console.WriteLine(Lang.Message.Done.PressEnterToExit);
                //Console.ReadLine();
                Console.WriteLine("The console will close automatically");
                Thread.Sleep(5000);
            }
            else
            {
                Console.WriteLine(Lang.Message.Done.PressEnterToExit);
                Console.ReadLine();
            }
        }

        private static void Run(String[] args)
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
            String executablePath = Assembly.GetEntryAssembly().Location;
            using (FileStream inputFile = File.OpenRead(executablePath))
            {
                Int64 magicNumber;
                BinaryReader br = new BinaryReader(inputFile);

                Assembly assembly = Assembly.GetExecutingAssembly();
                Module module = assembly.GetModules().First();
                X509Certificate certificate = module.GetSignerCertificate();
                // certificates vary slightly in size but are always within a few bytes this range is small enough that it's quick but will find the magic number if it's there
                // how to sign this patcher once it's completly built run from Output directory
                // signtool sign /d "Memoria Patcher for Modding FF9" /td SHA256 /fd SHA256 /sha1 {your certificates SHA1 signature} /tr http://timestamp.digicert.com .\Memoria.Patcher.exe 
                if (certificate != null)
                {
                    Console.WriteLine("Memoria.Patcher is Digitally signed, please wait while we validate");
                    Int64 possition = -0x2920;
                    bool Found = false;
                    while (!Found && possition < -0x2700)
                    {
                        inputFile.Seek(possition, SeekOrigin.End);
                        magicNumber = br.ReadInt64();
                        if (magicNumber == 0x004149524F4D454D)
                        {
                            Found = true;
                            break;
                        }
                        possition += 1;
                    }
                    if (!Found)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File is Signed but could not find magic number, file might be corrupted");
                        Console.ResetColor();
                        throw new InvalidDataException("File is Signed but could not find magic number, file might be corrupted");
                    }
                }
                else
                {
                    // if the file is not signed
                    inputFile.Seek(-0x18, SeekOrigin.End);

                    magicNumber = br.ReadInt64();
                    if (magicNumber != 0x004149524F4D454D)// MEMORIA\0 
                        throw new InvalidDataException("Invalid magic number: " + magicNumber);
                }
                //Console.Clear();
                try
                {
                    Int64 uncompressedDataSize = br.ReadInt64();
                    Int64 compressedDataPosition = br.ReadInt64();

                    Boolean isSteamOverlayFixed = GameLocationSteamRegistryProvider.IsSteamOverlayFixed();
                    Boolean fixReleased = false;
                    if (isSteamOverlayFixed)
                        fixReleased = gameLocation.FixSteamOverlay(false); // Release the registry lock

                    inputFile.Position = compressedDataPosition;
                    using (ConsoleProgressHandler progressHandler = new ConsoleProgressHandler(uncompressedDataSize))
                    using (GZipStream input = new GZipStream(inputFile, CompressionMode.Decompress))
                    using (br = new BinaryReader(input))
                    {
                        Int64 leftSize = uncompressedDataSize;
                        ExtractFiles(gameLocation, input, br, ref leftSize, progressHandler);
                        for (Int32 i = 0; i < 5000; i = i + 50)
                        {
                            if (progressHandler.CurrentPercent == 100)
                                i = 10000;
                            else
                                Thread.Sleep(50);
                        }
                        ProgressPercent = progressHandler.CurrentPercent;
                    }

                    if (isSteamOverlayFixed && fixReleased)
                        gameLocation.FixSteamOverlay(true); // Backup and redo the registry lock
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("A Patching error has occurred");
                    Console.ResetColor();
                    Console.WriteLine("---------------------------");
                    Console.WriteLine(ex);
                    Console.WriteLine("---------------------------");
                }
            }
        }

        private static void ExtractFiles(GameLocationInfo gameLocation, GZipStream input, BinaryReader br, ref Int64 leftSize, ConsoleProgressHandler progressHandler)
        {
            try
            {
                Dictionary<Int16, String> pathMap = new Dictionary<Int16, String>(400);
                UInt16 idMask = 1 << 15;

                Byte[] buff = new Byte[64 * 1024];

                while (leftSize > 0)
                {
                    Int64 uncompressedSize = br.ReadUInt32();
                    DateTime writeTimeUtc = new DateTime(br.ReadInt64(), DateTimeKind.Utc);

                    Boolean hasPlatform = false;
                    String[] pathParts = new String[br.ReadByte() + 1];
                    pathParts[0] = gameLocation.RootDirectory;
                    for (Int32 i = 1; i < pathParts.Length; i++)
                    {
                        String part = null;

                        Int16 id = br.ReadInt16();
                        if ((id & idMask) == idMask)
                        {
                            id = (Int16)(id & ~idMask);

                            Int32 bytesNumber = br.ReadByte();
                            Int32 readed = 0;
                            while (bytesNumber > 0)
                            {
                                readed = br.Read(buff, readed, bytesNumber);
                                bytesNumber -= readed;
                            }

                            part = Encoding.UTF8.GetString(buff, 0, readed);
                            pathParts[i] = part;
                            pathMap.Add(id, part);
                        }
                        else
                        {
                            part = pathMap[id];
                            pathParts[i] = part;
                        }

                        if (part == "{PLATFORM}")
                            hasPlatform = true;
                    }

                    String outputPath = Path.Combine(pathParts);

                    if (hasPlatform)
                    {
                        if (Directory.Exists(gameLocation.ManagedPathX64))
                        {
                            if (Directory.Exists(gameLocation.ManagedPathX86))
                            {
                                String x64 = outputPath.Replace("{PLATFORM}", "x64");
                                String x86 = outputPath.Replace("{PLATFORM}", "x86");
                                ExtractFile(input, uncompressedSize, buff, writeTimeUtc, progressHandler, x64, x86);
                            }
                            else
                            {
                                outputPath = outputPath.Replace("{PLATFORM}", "x86");
                                ExtractFile(input, uncompressedSize, buff, writeTimeUtc, progressHandler, outputPath);
                            }
                        }
                        else if (Directory.Exists(gameLocation.ManagedPathX86))
                        {
                            outputPath = outputPath.Replace("{PLATFORM}", "x86");
                            ExtractFile(input, uncompressedSize, buff, writeTimeUtc, progressHandler, outputPath);
                        }
                        else
                        {
                            progressHandler.IncrementProcessedSize(uncompressedSize);
                        }
                    }
                    else
                    {
                        ExtractFile(input, uncompressedSize, buff, writeTimeUtc, progressHandler, outputPath);
                    }

                    leftSize -= uncompressedSize;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An extraction error has occurred");
                Console.WriteLine("Try running the patcher as administrator");
                Console.ResetColor();
                Console.WriteLine("---------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("---------------------------");
            }
        }

        private static void ExtractFile(GZipStream input, Int64 uncompressedSize, Byte[] buff, DateTime writeTimeUtc, ConsoleProgressHandler progressHandler, params String[] outputPaths)
        {
            try {
                List<FileStream> outputs = new List<FileStream>(outputPaths.Length);
                Boolean isIni = outputPaths.Length > 0 && _iniFileName.Contains(Path.GetFileName(outputPaths[0]));
                Boolean success = false;
                try
                {
                    foreach (String outputPath in outputPaths)
                        outputs.Add(OverwriteFile(outputPath));

                    while (uncompressedSize > 0)
                    {
                        Int32 readed = input.Read(buff, 0, (Int32)Math.Min(uncompressedSize, buff.Length));
                        uncompressedSize -= readed;

                        foreach (FileStream output in outputs)
                            output.Write(buff, 0, readed);

                        progressHandler.IncrementProcessedSize(readed);
                    }

                    success = true;
                }
                finally
                {
                    foreach (FileStream output in outputs)
                        output.Dispose();
                }

                if (isIni && success && File.Exists(outputPaths[0]) && File.Exists(outputPaths[0] + ".bak"))
                {
                    File.WriteAllLines(outputPaths[0], MergeIniFiles(File.ReadAllLines(outputPaths[0]), File.ReadAllLines(outputPaths[0] + ".bak")));
                    File.Delete(outputPaths[0] + ".bak");
                }

                foreach (String outputPath in outputPaths)
                    File.SetLastWriteTimeUtc(outputPath, writeTimeUtc);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An extraction error has occurred for a file");
                Console.ResetColor();
                Console.WriteLine("---------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("---------------------------");
            }
        }

        private static readonly HashSet<String> _filesForBackup = new HashSet<String>(StringComparer.OrdinalIgnoreCase) { ".exe", ".dll" };
        private static readonly HashSet<String> _iniFileName = new HashSet<String>(StringComparer.OrdinalIgnoreCase) { "Memoria.ini", "Settings.ini" };

        private static FileStream OverwriteFile(String outputPath)
        {
            if (File.Exists(outputPath))
            {
                String extension = Path.GetExtension(outputPath);
                String outputName = Path.GetFileName(outputPath);
                if (_filesForBackup.Contains(extension))
                {
                    String backupPath = Path.ChangeExtension(outputPath, ".bak");
                    if (!File.Exists(backupPath))
                        File.Move(outputPath, backupPath);
                }
                else if (_iniFileName.Contains(outputName))
                {
                    String backupPath = outputPath + ".bak";
                    if (!File.Exists(backupPath))
                        File.Move(outputPath, backupPath);
                }
            }
            else
            {
                String directoryName = Path.GetDirectoryName(outputPath);
                if (directoryName != null)
                    Directory.CreateDirectory(directoryName);
            }

            return File.Create(outputPath);
        }

        private static String[] MergeIniFiles(String[] newIni, String[] previousIni)
        {
            List<String> mergedIni = new List<String>(previousIni.Where(line => !line.StartsWith("\t; "))); // In order to have a persisting custom comment, the user must use a slightly different format than "	; " (eg. "	;; ")
            for (Int32 i = 0; i < mergedIni.Count; i++)
            {
                // Hotfix: replace incorrect default formulas by the correct ones
                if (String.Compare(mergedIni[i], "StatusDurationFormula = ContiCnt * (IsNegativeStatus ? 8 * (60 - TargetSpirit) : 8 * TargetSpirit)") == 0
                    || String.Compare(mergedIni[i], "StatusTickFormula = OprCnt * (IsNegativeStatus ? 4 * (60 - TargetSpirit) : 4 * TargetSpirit)") == 0)
                {
                    mergedIni.RemoveAt(i--);
                }
                // Make sure spaces are present around =
                if (!mergedIni[i].Trim().StartsWith(";"))
                {
                    var split = mergedIni[i].Split('=');
                    for (Int32 j = 0; j < split.Length; j++)
                    {
                        split[j] = split[j].Trim();
                    }
                    mergedIni[i] = String.Join(" = ", split);
                }
            }
            String currentSection = "";
            Int32 sectionFirstLine = 0;
            Int32 sectionLastLine = 0;
            foreach (String line in newIni)
            {
                String trimmedLine = line.Trim();
                if (trimmedLine.Length == 0)
                    continue;
                if (trimmedLine.StartsWith("["))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.IndexOf("]") - 1);
                    sectionFirstLine = mergedIni.FindIndex(s => s.Trim().StartsWith("[" + currentSection + "]"));
                    Boolean hasSection = sectionFirstLine >= 0;
                    if (hasSection)
                    {
                        sectionFirstLine++;
                        sectionLastLine = sectionFirstLine;
                        while (sectionLastLine < mergedIni.Count)
                        {
                            if (mergedIni[sectionLastLine].Trim().StartsWith("["))
                                break;
                            sectionLastLine++;
                        }
                        while (sectionLastLine > 0 && mergedIni[sectionLastLine - 1].Trim().Length == 0)
                            sectionLastLine--;
                    }
                    else
                    {
                        mergedIni.Add("");
                        mergedIni.Add("[" + currentSection + "]");
                        sectionFirstLine = mergedIni.Count;
                        sectionLastLine = sectionFirstLine;
                    }
                }
                else if (trimmedLine.StartsWith(";"))
                {
                    if (!mergedIni.Exists(s => s.Trim() == trimmedLine))
                    {
                        mergedIni.Insert(sectionFirstLine, line);
                        sectionFirstLine++;
                        sectionLastLine++;
                    }
                }
                else
                {
                    String fieldName = trimmedLine.Substring(0, trimmedLine.IndexOfAny(new Char[] { ' ', '\t', '=' }));
                    Boolean fieldKnown = false;
                    for (Int32 i = sectionFirstLine; i < sectionLastLine; i++)
                    {
                        if (mergedIni[i].Trim().StartsWith(fieldName))
                        {
                            fieldKnown = true;
                            break;
                        }
                    }
                    if (!fieldKnown)
                    {
                        mergedIni.Insert(sectionLastLine, line);
                        sectionLastLine++;
                    }
                }
            }
            return mergedIni.ToArray();
        }

        private static GameLocationInfo GetGameLocation(String[] args)
        {
            try
            {
                GameLocationInfo result;
                if (args == null || args.Length == 0)
                {
                    if (File.Exists(GameLocationInfo.LauncherName))
                    {
                        result = new GameLocationInfo(Environment.CurrentDirectory);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("V ");
                        Console.ResetColor();
                        Console.WriteLine($"FF9_Launcher.exe found in Patcher directory directory {Environment.CurrentDirectory}");
                        result.Validate();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("V ");
                        Console.ResetColor();
                        Console.WriteLine($"Trying to find directory from Registry");
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to get a game location");
                Console.ResetColor();
                Console.WriteLine("If you changed the game directory from your first install, install this from the new game location.");
                Console.WriteLine("Make sure you have launched the game at least once.");
                Console.WriteLine("---------------------------");
                Console.WriteLine(ex);
                Console.WriteLine("---------------------------");
                return null;
            }
        }
    }
}
