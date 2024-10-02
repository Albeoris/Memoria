using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Memoria.Launcher
{
    // unused for now //
    public partial class ModConflictWindow : Window, IComponentConnector
    {
        public ObservableCollection<Mod> ModInstalledList;
        public List<Mod> ModMainList;
        public List<Mod> ModFullList;

        public ModConflictWindow(ObservableCollection<Mod> mods, Boolean activeOnly)
        {
            InitializeComponent();
            ModInstalledList = mods;
            GenerateConflictInformations(activeOnly);
        }

        private void OnModSelection(Object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            ModConflict selectedMod = lb?.SelectedItem as ModConflict;
            DetailedInfos.Text = selectedMod?.DetailedInfo;
        }

        private void OnButtonClick(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static void ScanModFileList(Mod mod)
        {
            if (mod.FileContent.Count > 0)
                return;
            if (!File.Exists(mod.FullInstallationPath + "/" + Mod.MOD_CONTENT_FILE))
            {
                GenerateModFileList(mod);
            }
            else
            {
                mod.FileContent = new HashSet<String>();
                String listPath = mod.FullInstallationPath + "/" + Mod.MOD_CONTENT_FILE;
                foreach (String line in File.ReadAllLines(listPath))
                {
                    String trimmedLine = line.Trim().ToLower();
                    if (trimmedLine.Length == 0)
                        continue;
                    if (trimmedLine.Contains('<'))
                        continue;
                    mod.FileContent.Add(trimmedLine);
                }
            }
        }

        private static void GenerateModFileList(Mod mod)
        {
            // It doesn't seem possible to deal with UnityEngine.AssetBundle in the launcher, even using Reflection
            // Besides, it's probably not a good idea to add a DLL dependancy to UnityEngine (or worse to Assembly-CSharp) inside the launcher, that should remain with minimal resource needs
            // Thus, when there's a AssetBundle, let Assembly-CSharp's AssetManager.GenerateFileList create the mod file list
            Dictionary<Mod, Dictionary<String, String>> allLists = new Dictionary<Mod, Dictionary<String, String>>();
            List<Mod> modAndSubs = new List<Mod>(mod.SubMod);
            modAndSubs.Add(mod);
            foreach (Mod anyMod in modAndSubs)
            {
                allLists[anyMod] = new Dictionary<String, String>();
                allLists[anyMod][String.Empty] = "";
            }
            Mod belongingMod;
            foreach (String filePath in Directory.EnumerateFiles(mod.InstallationPath, "*", SearchOption.AllDirectories))
            {
                String shortPath = filePath.Substring(mod.InstallationPath.Length + 1).Replace("\\", "/").ToLower();
                belongingMod = mod;
                foreach (Mod subMod in mod.SubMod)
                {
                    if (shortPath.StartsWith(subMod.InstallationPath + "/", StringComparison.OrdinalIgnoreCase))
                    {
                        shortPath = shortPath.Substring(subMod.InstallationPath.Length + 1);
                        belongingMod = subMod;
                        break;
                    }
                }
                if (shortPath.StartsWith("streamingassets/"))
                    shortPath = shortPath.Substring("streamingassets/".Length);
                else if (shortPath.StartsWith("ff9_data/"))
                    shortPath = shortPath.Substring("ff9_data/".Length);
                else if (!Mod.MEMORIA_ROOT_FILES.Contains(shortPath))
                    continue;
                if (Mod.ARCHIVE_BUNDLE_FILES.Contains(shortPath)) // Abort
                    return;
                allLists[belongingMod][String.Empty] += shortPath + "\n";
            }
            foreach (Mod anyMod in modAndSubs)
            {
                String listPath = anyMod.FullInstallationPath + "/" + Mod.MOD_CONTENT_FILE;
                String completeList = allLists[anyMod][String.Empty];
                foreach (KeyValuePair<String, String> kvp in allLists[anyMod])
                    if (kvp.Key != String.Empty)
                        completeList += $"<{kvp.Key}>\n" + kvp.Value;
                File.WriteAllText(listPath, completeList);
            }
            ScanModFileList(mod);
        }

        private static IEnumerable<String> ComputeFilesWithConflict(Mod mod1, Mod mod2)
        {
            ScanModFileList(mod1);
            ScanModFileList(mod2);
            if (mod1.FileContent.Count < mod2.FileContent.Count)
                return mod1.FileContent.Where(filePath => mod2.FileContent.Contains(filePath));
            return mod2.FileContent.Where(filePath => mod1.FileContent.Contains(filePath));
        }

        private void GenerateConflictInformations(Boolean checkActiveOnly)
        {
            ModConflictList.Clear();
            ModMainList = checkActiveOnly ? new List<Mod>(ModInstalledList.Where(mod => mod.IsActive)) : new List<Mod>(ModInstalledList);
            ModFullList = new List<Mod>();
            foreach (Mod mod in ModMainList)
            {
                ScanModFileList(mod);
                ModFullList.AddRange(mod.EnumerateModAndSubModsOrdered(checkActiveOnly));
            }
            HashSet<String> overwrittenFiles = new HashSet<String>();
            for (Int32 i = 0; i < ModFullList.Count; i++)
            {
                ModConflictList.Add(new ModConflict(ModFullList[i], $"✔ {ModFullList[i].Name}"));
                overwrittenFiles.Clear();
                String noteStr = "";
                if (!ModConflictList[i].HasFileList)
                {
                    ModConflictList[i].Label = $"? {ModFullList[i].Name}";
                    noteStr += $"[{ModFullList[i].Name}] File list is missing and cannot be automatically generated: the details of file-by-file potential conflicts is not displayed.\n";
                }
                if (!String.IsNullOrEmpty(ModFullList[i].CompatibilityNotes.GenericNotes))
                    noteStr += ModFullList[i].CompatibilityNotes.GenericNotes + "\n";
                for (Int32 j = 0; j < i; j++)
                {
                    if (ModFullList[i].CompatibilityNotes.OtherModHigh.TryGetValue(ModFullList[j].InstallationPath, out String specificNoteHigh))
                        noteStr += $"[{ModFullList[i].Name}/{ModFullList[j].Name}] {specificNoteHigh}\n";
                    if (ModFullList[j].CompatibilityNotes.OtherModLow.TryGetValue(ModFullList[i].InstallationPath, out String specificNoteLow))
                        noteStr += $"[{ModFullList[j].Name}/{ModFullList[i].Name}] {specificNoteLow}\n";
                    if (ModFullList[i].ParentMod == ModFullList[j] || ModFullList[j].ParentMod == ModFullList[i])
                        continue;
                    List<String> commonFiles = new List<String>(ComputeFilesWithConflict(ModFullList[i], ModFullList[j]).Where(path => !overwrittenFiles.Contains(path)));
                    Boolean hasConflict = false;
                    foreach (String path in commonFiles)
                    {
                        String pathLower = path.ToLower();
                        if (String.Compare(pathLower, "memoria.ini") == 0)
                        {
                            if (HandleSpecialFileConflicts(ModFullList[i].FullInstallationPath + "/" + path, ModFullList[j].FullInstallationPath + "/" + path, EnumerateIniConflicts, 5, out String entryStr))
                            {
                                hasConflict = true;
                                ModConflictList[i].DetailedInfo += $"{ModFullList[j].Name} supplants the Memoria.ini opton(s) {entryStr}\n";
                            }
                        }
                        else if (String.Compare(pathLower, "dictionarypatch.txt") == 0 || String.Compare(pathLower, "battlepatch.txt") == 0 || String.Compare(pathLower, "battlevoiceeffects.txt") == 0)
                        {
                            // TODO
                            hasConflict = true;
                            ModConflictList[i].DetailedInfo += $"{ModFullList[j].Name} might override options of '{path}'\n";
                        }
                        else if (String.Compare(pathLower, "data/text/localizationpatch.txt") == 0)
                        {
                            if (HandleSpecialFileConflicts(ModFullList[i].FullInstallationPath + "/StreamingAssets/" + path, ModFullList[j].FullInstallationPath + "/StreamingAssets/" + path, EnumerateLocalizationConflicts, 5, out String entryStr))
                            {
                                hasConflict = true;
                                ModConflictList[i].DetailedInfo += $"{ModFullList[j].Name} supplants the 'Localization' text(s) {entryStr}\n";
                            }
                        }
                        else if (CsvWithPartialData.Contains(pathLower))
                        {
                            if (HandleSpecialFileConflicts(ModFullList[i].FullInstallationPath + "/StreamingAssets/" + path, ModFullList[j].FullInstallationPath + "/StreamingAssets/" + path, EnumerateCsvConflicts, 5, out String entryStr))
                            {
                                hasConflict = true;
                                ModConflictList[i].DetailedInfo += $"{ModFullList[j].Name} supplants the '{path}' entry(ies) {entryStr}\n";
                            }
                        }
                        else
                        {
                            // TODO: text files (.mes) that can be cumulated with [TXID=***]
                            hasConflict = true;
                            overwrittenFiles.Add(path);
                            ModConflictList[i].DetailedInfo += $"{ModFullList[j].Name} supplants '{path}'\n";
                        }
                    }
                    if (!hasConflict)
                        continue;
                    if (!ModConflictList[i].HasConflicts)
                        ModConflictList[i].Label = $"✘ {ModFullList[i].Name} partly supplanted by {ModFullList[j].Name}";
                    else
                        ModConflictList[i].Label += $" - {ModFullList[j].Name}";
                    ModConflictList[i].HasConflicts = true;
                }
                if (!String.IsNullOrEmpty(noteStr))
                    ModConflictList[i].DetailedInfo = noteStr + "\n" + ModConflictList[i].DetailedInfo;
            }
            if (!ModConflictList.Any(c => c.HasConflicts))
                OverallVerdict.Text = "No conflict found";
            else
                OverallVerdict.Text = "There may be some mod conflicts";
            ModList.ItemsSource = ModConflictList;
            DetailedInfos.Text = String.Empty;
            ModList.SelectedItem = null;
        }

        private Boolean HandleSpecialFileConflicts(String path1, String path2, SpecialFileConflictEnumerator enumerator, Int32 maxCount, out String joinedEntries)
        {
            List<String> commonEntries = new List<String>();
            joinedEntries = "";
            foreach (String str in enumerator(path1, path2))
            {
                commonEntries.Add(str);
                if (commonEntries.Count >= maxCount)
                {
                    commonEntries.Add("...");
                    break;
                }
            }
            if (commonEntries.Count == 0)
                return false;
            joinedEntries = String.Join(", ", commonEntries.ToArray());
            return true;
        }

        public IEnumerable<String> EnumerateIniConflicts(String iniPath1, String iniPath2)
        {
            String[] iniFile1;
            String[] iniFile2;
            try
            {
                iniFile1 = File.ReadAllLines(iniPath1);
                iniFile2 = File.ReadAllLines(iniPath2);
            }
            catch (Exception)
            {
                yield break;
            }
            Dictionary<String, List<String>> sectionSorted2 = new Dictionary<String, List<String>>();
            String currentSectionStr = String.Empty;
            List<String> currentSection2 = null;
            foreach (String line2 in iniFile2)
            {
                String trimmedLine = line2.Trim();
                if (trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    continue;
                if (trimmedLine.StartsWith("["))
                {
                    Int32 endBracket = trimmedLine.IndexOf(']');
                    if (endBracket < 0)
                        continue;
                    currentSectionStr = trimmedLine.Substring(1, endBracket - 1);
                    if (!sectionSorted2.TryGetValue(currentSectionStr, out currentSection2))
                        sectionSorted2[currentSectionStr] = currentSection2 = new List<String>();
                }
                else if (currentSection2 != null)
                {
                    String fieldName = String.Concat(trimmedLine.TakeWhile(ch => Char.IsLetterOrDigit(ch)));
                    currentSection2.Add(fieldName);
                }
            }
            currentSection2 = null;
            foreach (String line1 in iniFile1)
            {
                String trimmedLine = line1.Trim();
                if (trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    continue;
                if (trimmedLine.StartsWith("["))
                {
                    Int32 endBracket = trimmedLine.IndexOf(']');
                    if (endBracket < 0)
                        continue;
                    currentSectionStr = trimmedLine.Substring(1, endBracket - 1);
                    if (!sectionSorted2.TryGetValue(currentSectionStr, out currentSection2))
                        currentSection2 = null;
                }
                else if (currentSection2 != null)
                {
                    String fieldName = String.Concat(trimmedLine.TakeWhile(ch => Char.IsLetterOrDigit(ch)));
                    if (currentSection2.Contains(fieldName))
                        yield return $"[{currentSectionStr}] {fieldName}";
                }
            }
        }

        public IEnumerable<String> EnumerateLocalizationConflicts(String locPath1, String locPath2)
        {
            String[] locFile1;
            String[] locFile2;
            try
            {
                locFile1 = File.ReadAllLines(locPath1);
                locFile2 = File.ReadAllLines(locPath2);
            }
            catch (Exception)
            {
                yield break;
            }

            foreach (String line1 in locFile1)
            {
                Int32 commaIndex = line1.IndexOf(',');
                if (commaIndex <= 0)
                    continue;
                String entry = line1.Substring(0, commaIndex);
                String searchStr = entry + ",";
                if (locFile2.Any(line2 => line2.StartsWith(searchStr)))
                    yield return entry;
            }
        }

        public IEnumerable<String> EnumerateCsvConflicts(String csvPath1, String csvPath2)
        {
            String[] csvFile1;
            String[] csvFile2;
            try
            {
                csvFile1 = File.ReadAllLines(csvPath1);
                csvFile2 = File.ReadAllLines(csvPath2);
            }
            catch (Exception)
            {
                yield break;
            }
            String csvName = Path.GetFileName(csvPath1);
            List<Int32> entryId2 = new List<Int32>();
            Int32 entryCounter = 0;
            if (!DefaultCsvIdIndex.TryGetValue(csvName, out Int32 idIndex))
                idIndex = -1;
            foreach (String line in csvFile2)
                if (ProcessCsvLine(csvName, line, ref entryCounter, ref idIndex, out Int32 id))
                    entryId2.Add(id);
            entryCounter = 0;
            if (!DefaultCsvIdIndex.TryGetValue(csvName, out idIndex))
                idIndex = -1;
            foreach (String line in csvFile1)
                if (ProcessCsvLine(csvName, line, ref entryCounter, ref idIndex, out Int32 id) && entryId2.Contains(id))
                    yield return id.ToString();
        }

        private Boolean ProcessCsvLine(String csvName, String line, ref Int32 entryCounter, ref Int32 idIndex, out Int32 id)
        {
            id = -1;
            if (String.IsNullOrEmpty(line))
                return false;
            if (line.StartsWith("#!"))
            {
                idIndex = GetModifiedCsvIdIndex(csvName, line.Substring(2).TrimStart(), idIndex);
                return false;
            }
            if (line[0] == '#')
                return false;
            String[] entries = line.Split(';');
            for (Int32 i = 0; i < entries.Length; i++)
            {
                String col = entries[i];
                if (col.Length > 0 && col[0] == '#')
                {
                    Array.Resize(ref entries, i);
                    break;
                }
            }
            if (idIndex < 0 || idIndex >= entries.Length || !Int32.TryParse(entries[idIndex], out id))
                id = entryCounter;
            entryCounter++;
            return true;
        }

        private Int32 GetModifiedCsvIdIndex(String csvName, String metaDataInstruction, Int32 currentIndex)
        {
            switch (metaDataInstruction + ":" + csvName)
            {
                case "IncludeId:commands.csv":
                case "IncludeId:commandsets.csv":
                case "IncludeId:itemeffects.csv":
                case "IncludeId:items.csv":
                    return 0;
            }
            return currentIndex;
        }

        private class ModConflict
        {
            public Mod BaseMod { get; set; }
            public String Label { get; set; }
            public String DetailedInfo { get; set; }
            public Boolean HasConflicts { get; set; }
            public Boolean HasFileList { get; set; }

            public ModConflict(Mod mod, String label)
            {
                BaseMod = mod;
                Label = label;
                DetailedInfo = String.Empty;
                HasConflicts = false;
                HasFileList = File.Exists(BaseMod.FullInstallationPath + "/" + Mod.MOD_CONTENT_FILE);
            }
        }

        private delegate IEnumerable<String> SpecialFileConflictEnumerator(String path1, String path2);

        private List<ModConflict> ModConflictList = new List<ModConflict>();

        private static readonly HashSet<String> CsvWithPartialData = new HashSet<String>()
        {
            "data/battle/actions.csv",
            "data/battle/magicswordsets.csv",
            "data/battle/statusdata.csv",
            "data/battle/statussets.csv",
            "data/characters/abilities/abilitygems.csv",
            "data/characters/basestats.csv",
            "data/characters/battleparameters.csv",
            "data/characters/characterparameters.csv",
            "data/characters/commands.csv",
            "data/characters/commandsets.csv",
            "data/characters/commandtitles.csv",
            "data/characters/defaultequipment.csv",
            "data/items/armors.csv",
            "data/items/itemeffects.csv",
            "data/items/items.csv",
            "data/items/shopitems.csv",
            "data/items/stats.csv",
            "data/items/synthesis.csv",
            "data/items/weapons.csv"
        };

        private static readonly Dictionary<String, Int32> DefaultCsvIdIndex = new Dictionary<String, Int32>()
        {
            { "actions.csv", 1 },
            { "magicswordsets.csv", 0 },
            { "statusdata.csv", 1 },
            { "statussets.csv", 1 },
            { "abilitygems.csv", 1 },
            { "basestats.csv", 1 },
            { "battleparameters.csv", 0 },
            { "characterparameters.csv", 0 },
            { "commandtitles.csv", 1 },
            { "defaultequipment.csv", 1 },
            { "armors.csv", 1 },
            { "shopitems.csv", 1 },
            { "stats.csv", 1 },
            { "synthesis.csv", 1 },
            { "weapons.csv", 1 }
        };
    }
}
