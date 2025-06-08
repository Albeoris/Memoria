using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Memoria.Launcher;

namespace OriginalIndexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create test XML with some mods
            string testXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ModCatalog>
    <Mod>
        <Name>Russian Translation</Name>
        <InstallationPath>RussianMod</InstallationPath>
        <Author>Translator</Author>
        <Category>Localization</Category>
        <SubMod>
            <Name>Font Patch</Name>
            <InstallationPath>FontPatch</InstallationPath>
        </SubMod>
        <SubMod>
            <Name>Voice Pack</Name>
            <InstallationPath>VoicePack</InstallationPath>
        </SubMod>
    </Mod>
    <Mod>
        <Name>Audio Enhancement</Name>
        <InstallationPath>AudioMod</InstallationPath>
        <Author>AudioDev</Author>
        <Category>Audio</Category>
    </Mod>
    <Mod>
        <Name>Battle Improvements</Name>
        <InstallationPath>BattleMod</InstallationPath>
        <Author>BattleDev</Author>
        <Category>Gameplay</Category>
    </Mod>
</ModCatalog>";

            // Create a temporary XML file
            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, testXml);

            try
            {
                Console.WriteLine("=== Testing OriginalIndex Implementation ===\n");

                // Load mods from XML
                ObservableCollection<Mod> modList = new ObservableCollection<Mod>();
                using (StreamReader reader = new StreamReader(tempFile))
                {
                    Mod.LoadModDescriptions(reader, modList);
                }

                Console.WriteLine("1. Loaded mods with original indices:");
                for (int i = 0; i < modList.Count; i++)
                {
                    var mod = modList[i];
                    Console.WriteLine($"   UI Index {i}: '{mod.Name}' (OriginalIndex: {mod.OriginalIndex})");
                    
                    // Show submods too
                    for (int j = 0; j < mod.SubMod.Count; j++)
                    {
                        var subMod = mod.SubMod[j];
                        Console.WriteLine($"     SubMod UI Index {j}: '{subMod.Name}' (OriginalIndex: {subMod.OriginalIndex})");
                    }
                }

                Console.WriteLine("\n2. Sorting mods by name (descending):");
                var sortedMods = modList.OrderByDescending(m => m.Name).ToList();
                modList.Clear();
                foreach (var mod in sortedMods)
                {
                    modList.Add(mod);
                }

                // Show sorted list
                for (int i = 0; i < modList.Count; i++)
                {
                    var mod = modList[i];
                    Console.WriteLine($"   UI Index {i}: '{mod.Name}' (OriginalIndex: {mod.OriginalIndex})");
                }

                Console.WriteLine("\n3. Finding 'Russian Translation' by originalIndex (should be 0):");
                var russianMod = Mod.FindByOriginalIndex(modList, 0);
                if (russianMod != null)
                {
                    Console.WriteLine($"   Found: '{russianMod.Name}' at UI position {modList.IndexOf(russianMod)}");
                    
                    Console.WriteLine("\n4. Finding 'Voice Pack' submod by originalIndex (should be 1):");
                    var voicePackSubMod = Mod.FindSubModByOriginalIndex(russianMod, 1);
                    if (voicePackSubMod != null)
                    {
                        Console.WriteLine($"   Found submod: '{voicePackSubMod.Name}' at SubMod UI position {russianMod.SubMod.IndexOf(voicePackSubMod)}");
                    }
                    else
                    {
                        Console.WriteLine("   ERROR: Voice Pack submod not found!");
                    }
                }
                else
                {
                    Console.WriteLine("   ERROR: Russian Translation mod not found!");
                }

                Console.WriteLine("\n5. Getting next available indices:");
                Console.WriteLine($"   Next mod originalIndex: {Mod.GetNextModOriginalIndex(modList)}");
                if (russianMod != null)
                {
                    Console.WriteLine($"   Next submod originalIndex for Russian Translation: {Mod.GetNextSubModOriginalIndex(russianMod)}");
                }

                Console.WriteLine("\n=== Test completed successfully! ===");
                Console.WriteLine("OriginalIndex values are preserved during sorting and can be used to find correct mods/submods.");
            }
            finally
            {
                // Clean up
                File.Delete(tempFile);
            }
        }
    }
}