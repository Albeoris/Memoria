using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Memoria.Launcher
{
    public class Mod
    {
        // Entries that can be defined in .xml descriptions
        public String Name { get; set; }
        public Version CurrentVersion { get; set; }
        public Boolean IsOutdated { get; set; }
        public String UpdateIcon { get; set; }
        public String UpdateTooltip { get; set; }
        public String ReleaseDate { get; set; }
        public String ReleaseDateOriginal { get; set; }
        public String Author { get; set; }
        public String Description { get; set; }
        public String PatchNotes { get; set; }
        public CompatibilityNoteClass CompatibilityNotes { get; set; }
        public String IncompatibleWith { get; set; }
        public String IncompIcon { get; set; }
        public String ActiveIncompatibleMods { get; set; }
        public String HasPriorityOverMods { get; set; }
        public String Category { get; set; }
        public String Website { get; set; }
        public String DownloadUrl { get; set; }
        public String InstallationPath { get; set; }
        public String DownloadFormat { get; set; }
        public String PreviewFile { get; set; }
        public String PreviewFileUrl { get; set; }
        public String LauncherColor { get; set; }
        public String LauncherBackground { get; set; }
        public Int64 DownloadSize { get; set; } // TODO
        public Int64 FullSize { get; set; } // TODO
        public List<Mod> SubMod { get; set; }
        public String MinimumMemoriaVersion { get; set; }

        // Entries for sub-mods only
        public Mod ParentMod { get; set; }
        public String Group { get; set; }
        public Boolean IsHeader { get; set; }
        public Boolean IsDefault { get; set; }
        public List<String> ActivateWithMod { get; set; }
        public List<String> ActivateWithoutMod { get; set; }

        // Entries that are computed in the Mod Manager
        public HashSet<String> FileContent { get; set; }
        public Boolean IsActive { get; set; }
        public String Installed { get; set; }
        public Int32 Priority { get; set; }
        public BitmapImage PreviewImage { get; set; }
        public IniFile PresetIni { get; set; }

        // Entries for current download
        public String DownloadSpeed { get; set; }
        public String RemainingTime { get; set; }
        public Int64 PercentComplete { get; set; }

        public String FullInstallationPath => ParentMod != null ? ParentMod.InstallationPath + "/" + InstallationPath : InstallationPath;

        private const String presetFile = "Preset.ini";

        public Mod()
        {
            CompatibilityNotes = new CompatibilityNoteClass();
            FileContent = new HashSet<String>();
            SubMod = new List<Mod>();
        }

        public Mod(String name, String path)
        {
            Name = name?.Trim();
            InstallationPath = path;
            CompatibilityNotes = new CompatibilityNoteClass();
            FileContent = new HashSet<String>();
            SubMod = new List<Mod>();
        }

        public Mod(StreamReader reader)
        {
            CompatibilityNotes = new CompatibilityNoteClass();
            FileContent = new HashSet<String>();
            SubMod = new List<Mod>();
            ReadDescription(reader);
        }

        public Mod(String folderPath)
        {
            CompatibilityNotes = new CompatibilityNoteClass();
            FileContent = new HashSet<String>();
            SubMod = new List<Mod>();
            try
            {
                using (Stream input = File.OpenRead(folderPath + "/" + DESCRIPTION_FILE))
                using (StreamReader reader = new StreamReader(input))
                    ReadDescription(reader);
                if (InstallationPath == null || Path.GetFullPath(InstallationPath) != Path.GetFullPath(folderPath))
                    InstallationPath = folderPath.Replace(@".\", "");
            }
            catch (Exception)
            {
            }
        }

        public IEnumerable<Mod> EnumerateModAndSubModsOrdered(Boolean activeOnly)
        {
            return EnumerateModAndSubModsOrdered_Generic(false, activeOnly).Select(obj => obj as Mod);
        }

        public IEnumerable<String> EnumerateModAndSubModFoldersOrdered(Boolean activeOnly)
        {
            return EnumerateModAndSubModsOrdered_Generic(true, activeOnly).Select(obj => obj as String);
        }

        private IEnumerable<Object> EnumerateModAndSubModsOrdered_Generic(Boolean returnPaths, Boolean activeOnly)
        {
            if (activeOnly && !IsActive)
                yield break;
            Int32 subModIndex = 0;
            List<Mod> mods = new List<Mod>(SubMod);
            mods.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            while (subModIndex < mods.Count && mods[subModIndex].Priority >= 0)
            {
                if (!activeOnly || mods[subModIndex].IsActive)
                    yield return returnPaths ? mods[subModIndex].FullInstallationPath : mods[subModIndex];
                subModIndex++;
            }
            yield return returnPaths ? InstallationPath : this;
            while (subModIndex < mods.Count)
            {
                if (!activeOnly || mods[subModIndex].IsActive)
                    yield return returnPaths ? mods[subModIndex].FullInstallationPath : mods[subModIndex];
                subModIndex++;
            }
        }

        public static void LoadModDescriptions(StreamReader reader, ObservableCollection<Mod> modList)
        {
            XmlDocument doc = LoadDocument(reader);
            XmlNodeList rootNode = doc.SelectNodes("/ModCatalog");
            if (rootNode.Count != 1)
                return;
            XmlNodeList modNodes = rootNode[0].SelectNodes("Mod");
            foreach (XmlNode node in modNodes)
            {
                Mod mod = new Mod();
                if (mod.ReadDescription(node))
                    modList.Add(mod);
            }
        }

        public Boolean ReadDescription(StreamReader reader)
        {
            XmlDocument doc = LoadDocument(reader);
            XmlNodeList modList = doc.SelectNodes("/Mod");
            if (modList.Count != 1)
                return false;
            return ReadDescription(modList[0]);
        }

        public static XmlDocument LoadDocument(StreamReader reader)
        {
            String fullText = reader.ReadToEnd();
            // Fixes issue when '&' is in the url instead of '&amp;'
            fullText = Regex.Replace(fullText, @"&(?!\w+;)", "&amp;");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(fullText);
            return doc;
        }

        public Boolean ReadDescription(XmlNode modNode)
        {
            Regex indentRegex = new Regex(@"^[\t ]+", RegexOptions.Multiline);
            XmlElement elName = modNode["Name"];
            XmlElement elInstPath = modNode["InstallationPath"];
            if (elName == null || elInstPath == null)
                return false;
            XmlElement elVer = modNode["Version"];
            Int64 outParse;
            Name = elName.InnerText?.Trim();
            InstallationPath = elInstPath.InnerText;
            if (elVer != null && Version.TryParse(Regex.Replace(elVer.InnerText, @"[^\d\.]", ""), out Version version))
                CurrentVersion = version;
            ReleaseDate = modNode["ReleaseDate"]?.InnerText;
            ReleaseDateOriginal = modNode["ReleaseDateOriginal"]?.InnerText;
            Author = modNode["Author"]?.InnerText;
            Description = modNode["Description"] != null ? indentRegex.Replace(modNode["Description"].InnerText, "") : null;
            PatchNotes = modNode["PatchNotes"] != null ? indentRegex.Replace(modNode["PatchNotes"].InnerText, "") : null;
            foreach (XmlElement elComp in modNode.SelectNodes("CompatibilityNotes"))
            {
                if (elComp.HasAttribute("OtherModLow") || elComp.HasAttribute("OtherModHigh"))
                {
                    if (elComp.HasAttribute("OtherModLow"))
                        CompatibilityNotes.OtherModLow[elComp.GetAttribute("OtherModLow")] = elComp.InnerText;
                    if (elComp.HasAttribute("OtherModHigh"))
                        CompatibilityNotes.OtherModHigh[elComp.GetAttribute("OtherModHigh")] = elComp.InnerText;
                }
                else
                {
                    CompatibilityNotes.GenericNotes = elComp.InnerText;
                }
            }
            IncompatibleWith = modNode["IncompatibleWith"]?.InnerText;
            HasPriorityOverMods = modNode["HasPriorityOverMods"]?.InnerText;
            if (Int64.TryParse(modNode["Priority"]?.InnerText ?? "0", out outParse))
                Priority = (Int32)outParse;
            else
                Priority = 0;
            Category = modNode["Category"]?.InnerText;
            Website = modNode["Website"]?.InnerText;
            DownloadUrl = modNode["DownloadUrl"]?.InnerText;
            DownloadFormat = modNode["DownloadFormat"]?.InnerText;
            PreviewFile = modNode["PreviewFile"]?.InnerText;
            PreviewFileUrl = modNode["PreviewFileUrl"]?.InnerText;
            MinimumMemoriaVersion = modNode["MinimumMemoriaVersion"]?.InnerText;
            LauncherColor = modNode["LauncherColor"]?.InnerText;
            LauncherBackground = modNode["LauncherBackground"]?.InnerText;
            if (Int64.TryParse(modNode["FullSize"]?.InnerText ?? "0", out outParse)) FullSize = outParse;
            if (Int64.TryParse(modNode["DownloadSize"]?.InnerText ?? "0", out outParse)) DownloadSize = outParse;
            if (File.Exists(Path.Combine(FullInstallationPath, presetFile)))
            {
                PresetIni = new IniFile(Path.Combine(FullInstallationPath, presetFile));
            }
            SubMod.Clear();
            foreach (XmlNode subNode in modNode.ChildNodes)
            {
                if (subNode.Name != "SubMod" && subNode.Name != "Header")
                    continue;

                Mod sub = new Mod();
                if (subNode.Name == "Header")
                {
                    sub.Name = subNode.InnerText?.Trim();
                    sub.IsHeader = true;
                    if (!String.IsNullOrEmpty(sub.Name))
                        SubMod.Add(sub);
                    continue;
                }
                elName = subNode["Name"];
                elInstPath = subNode["InstallationPath"];
                if (elName == null || elInstPath == null)
                    continue;
                sub.Name = elName.InnerText?.Trim();
                sub.InstallationPath = elInstPath.InnerText;
                sub.Description = subNode["Description"] != null ? indentRegex.Replace(subNode["Description"].InnerText, "") : null;
                sub.Category = subNode["Category"]?.InnerText;
                sub.PreviewFile = subNode["PreviewFile"]?.InnerText;
                sub.PreviewFileUrl = subNode["PreviewFileUrl"]?.InnerText;
                sub.LauncherColor = subNode["LauncherColor"]?.InnerText;
                sub.LauncherBackground = subNode["LauncherBackground"]?.InnerText;

                sub.Group = subNode["Group"]?.InnerText;
                sub.IsDefault = subNode["Default"] != null;
                if (subNode["ActivateWithMod"] != null)
                {
                    sub.ActivateWithMod = new List<string>();
                    foreach (String mod in subNode["ActivateWithMod"].InnerText.Split(','))
                        sub.ActivateWithMod.Add(mod.Trim());
                }
                else if (subNode["ActivateWithoutMod"] != null)
                {
                    sub.ActivateWithoutMod = new List<string>();
                    foreach (String mod in subNode["ActivateWithoutMod"].InnerText.Split(','))
                        sub.ActivateWithoutMod.Add(mod.Trim());
                }
                if (Int64.TryParse(subNode["Priority"]?.InnerText ?? "0", out outParse))
                    sub.Priority = (Int32)outParse;
                else
                    sub.Priority = 0;
                sub.ParentMod = this;
                SubMod.Add(sub);
            }
            return true;
        }

        public void GenerateDescription(String folderPath)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mod = doc.CreateNode(XmlNodeType.Element, "Mod", "");
            doc.AppendChild(mod);
            XmlElement el = doc.CreateElement("Name");
            el.InnerText = Name;
            mod.AppendChild(el);
            if (CurrentVersion != null)
            {
                el = doc.CreateElement("Version");
                el.InnerText = CurrentVersion.ToString();
                mod.AppendChild(el);
            }
            if (InstallationPath != null)
            {
                el = doc.CreateElement("InstallationPath");
                el.InnerText = InstallationPath;
                mod.AppendChild(el);
            }
            if (ReleaseDate != null)
            {
                el = doc.CreateElement("ReleaseDate");
                el.InnerText = ReleaseDate;
                mod.AppendChild(el);
            }
            if (ReleaseDateOriginal != null)
            {
                el = doc.CreateElement("ReleaseDateOriginal");
                el.InnerText = ReleaseDateOriginal;
                mod.AppendChild(el);
            }
            if (Author != null)
            {
                el = doc.CreateElement("Author");
                el.InnerText = Author;
                mod.AppendChild(el);
            }
            if (Description != null)
            {
                el = doc.CreateElement("Description");
                el.InnerText = Description;
                mod.AppendChild(el);
            }
            if (PatchNotes != null)
            {
                el = doc.CreateElement("PatchNotes");
                el.InnerText = PatchNotes;
                mod.AppendChild(el);
            }
            if (IncompatibleWith != null)
            {
                el = doc.CreateElement("IncompatibleWith");
                el.InnerText = IncompatibleWith;
                mod.AppendChild(el);
            }
            if (HasPriorityOverMods != null)
            {
                el = doc.CreateElement("HasPriorityOverMods");
                el.InnerText = HasPriorityOverMods;
                mod.AppendChild(el);
            }
            if (Category != null)
            {
                el = doc.CreateElement("Category");
                el.InnerText = Category;
                mod.AppendChild(el);
            }
            if (Website != null)
            {
                el = doc.CreateElement("Website");
                el.InnerText = Website;
                mod.AppendChild(el);
            }
            if (DownloadUrl != null)
            {
                el = doc.CreateElement("DownloadUrl");
                el.InnerText = DownloadUrl;
                mod.AppendChild(el);
            }
            if (DownloadFormat != null)
            {
                el = doc.CreateElement("DownloadFormat");
                el.InnerText = DownloadFormat;
                mod.AppendChild(el);
            }
            if (PreviewFile != null)
            {
                el = doc.CreateElement("PreviewFile");
                el.InnerText = PreviewFile;
                mod.AppendChild(el);
            }
            if (PreviewFileUrl != null)
            {
                el = doc.CreateElement("PreviewFileUrl");
                el.InnerText = PreviewFileUrl;
                mod.AppendChild(el);
            }
            if (MinimumMemoriaVersion != null)
            {
                el = doc.CreateElement("MinimumMemoriaVersion");
                el.InnerText = MinimumMemoriaVersion;
                mod.AppendChild(el);
            }
            if (FullSize > 0)
            {
                el = doc.CreateElement("FullSize");
                el.InnerText = FullSize.ToString();
                mod.AppendChild(el);
            }
            if (DownloadSize > 0)
            {
                el = doc.CreateElement("DownloadSize");
                el.InnerText = DownloadSize.ToString();
                mod.AppendChild(el);
            }
            if (SubMod != null)
            {
                foreach (Mod sub in SubMod)
                {
                    if (String.IsNullOrEmpty(sub.InstallationPath))
                    {
                        el = doc.CreateElement("Header");
                        el.InnerText = sub.Name;
                        mod.AppendChild(el);
                        continue;
                    }
                    el = doc.CreateElement("SubMod");
                    XmlElement subEl = doc.CreateElement("Name");
                    subEl.InnerText = sub.Name;
                    el.AppendChild(subEl);
                    subEl = doc.CreateElement("InstallationPath");
                    subEl.InnerText = sub.InstallationPath;
                    el.AppendChild(subEl);
                    if (sub.Description != null)
                    {
                        subEl = doc.CreateElement("Description");
                        subEl.InnerText = sub.Description;
                        el.AppendChild(subEl);
                    }
                    if (sub.Category != null)
                    {
                        subEl = doc.CreateElement("Category");
                        subEl.InnerText = sub.Category;
                        el.AppendChild(subEl);
                    }
                    if (sub.PreviewFile != null)
                    {
                        subEl = doc.CreateElement("PreviewFile");
                        subEl.InnerText = sub.PreviewFile;
                        el.AppendChild(subEl);
                    }
                    if (sub.PreviewFileUrl != null)
                    {
                        subEl = doc.CreateElement("PreviewFileUrl");
                        subEl.InnerText = sub.PreviewFileUrl;
                        el.AppendChild(subEl);
                    }
                    if (sub.Group != null)
                    {
                        subEl = doc.CreateElement("Group");
                        subEl.InnerText = sub.Group;
                        el.AppendChild(subEl);
                    }
                    if (sub.IsDefault)
                    {
                        subEl = doc.CreateElement("Default");
                        el.AppendChild(subEl);
                    }
                    if (sub.ActivateWithMod != null)
                    {
                        subEl = doc.CreateElement("ActivateWithMod");
                        subEl.InnerText = String.Join(", ", sub.ActivateWithMod);
                        el.AppendChild(subEl);
                    }
                    if (sub.ActivateWithoutMod != null)
                    {
                        subEl = doc.CreateElement("ActivateWithoutMod");
                        subEl.InnerText = String.Join(", ", sub.ActivateWithoutMod);
                        el.AppendChild(subEl);
                    }
                    if (sub.Priority != 0)
                    {
                        subEl = doc.CreateElement("Priority");
                        subEl.InnerText = sub.Priority.ToString();
                        el.AppendChild(subEl);
                    }
                    mod.AppendChild(el);
                }
            }
            File.WriteAllText(folderPath + "/" + DESCRIPTION_FILE, IndentXml(doc.OuterXml));
        }

        public void TryApplyPreset()
        {
            if (PresetIni == null) return;

            StringBuilder sb = new StringBuilder();
            IniFile.Key presetNameKey = new IniFile.Key("Preset", "Name");
            if (PresetIni.Options.ContainsKey(presetNameKey))
            {
                sb.AppendLine($"[{Name}] {PresetIni.Options[presetNameKey]}");
            }
            List<IniFile.Key> keys = new List<IniFile.Key>();
            foreach (var key in PresetIni.Options.Keys)
            {
                if (IniFile.MemoriaIni.Options.ContainsKey(key))
                {
                    keys.Add(key);
                    sb.AppendLine($"  [{key.Section}] {key.Name} = {PresetIni.Options[key]}");
                }
            }

            if (keys.Count == 0) return;

            if (MessageBox.Show($"{Lang.Res["ModEditor.ApplyModPresetText"]}\n\n{sb}", (String)Lang.Res["ModEditor.ApplyModPresetCaption"], MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                PresetIni.WriteAllSettings(IniFile.MemoriaIniPath, ["Preset"]);
                IniFile.MemoriaIni.Reload();

                MainWindow mainWindow = App.Current.MainWindow as MainWindow;
                mainWindow.LoadSettings();
            }
        }

        public static String IndentXml(String xmlRaw)
        {
            String indented = Regex.Replace(xmlRaw, @">(<[^/])", ">\n\t$1").Replace("</Mod>", "\n</Mod>").Replace("</SubMod>", "\n\t</SubMod>");
            String[] lines = indented.Split('\n');
            indented = "";
            Boolean inSubMod = false;
            Boolean firstLine = true;
            foreach (String line in lines)
            {
                if (line == "\t</SubMod>")
                    inSubMod = false;
                if (firstLine)
                    firstLine = false;
                else
                    indented += "\n";
                indented += inSubMod ? "\t" + line : line;
                if (line == "\t<SubMod>")
                    inSubMod = true;
            }
            return indented;
        }

        public static Mod SearchMod(IEnumerable<Mod> modList, Mod queryMod)
        {
            foreach (Mod mod in modList)
            {
                if (mod.InstallationPath != null && queryMod.InstallationPath != null && mod.InstallationPath == queryMod.InstallationPath)
                    return mod;
                else if (mod.Name == queryMod.Name)
                    return mod;
            }
            return null;
        }

        public static Mod SearchWithPath(IEnumerable<Mod> modList, String queryPath)
        {
            foreach (Mod mod in modList)
                if (mod.InstallationPath == queryPath)
                    return mod;
            return null;
        }

        public static Boolean LooksLikeAModFolder(String path)
        {
            if (File.Exists(path + "/" + Mod.DESCRIPTION_FILE))
                return true;
            if (File.Exists(path + "/Memoria.ini") || File.Exists(path + "/DictionaryPatch.txt"))
                return true;
            if (Directory.Exists(path + "/FF9_Data") || Directory.Exists(path + "/StreamingAssets"))
                return true;
            return false;
        }

        public class CompatibilityNoteClass
        {
            public String GenericNotes = String.Empty;
            public Dictionary<String, String> OtherModLow = new Dictionary<String, String>();
            public Dictionary<String, String> OtherModHigh = new Dictionary<String, String>();
        }

        public static readonly HashSet<String> MEMORIA_ROOT_FILES = new HashSet<String>()
        {
            "memoria.ini",
            "dictionarypatch.txt",
            "battlepatch.txt",
            "battlevoiceeffects.txt"
        };

        public static readonly HashSet<String> ARCHIVE_BUNDLE_FILES = new HashSet<String>()
        {
            "p0data11.bin",
            "p0data12.bin",
            "p0data13.bin",
            "p0data14.bin",
            "p0data15.bin",
            "p0data16.bin",
            "p0data17.bin",
            "p0data18.bin",
            "p0data19.bin",
            "p0data2.bin",
            "p0data3.bin",
            "p0data4.bin",
            "p0data5.bin",
            "p0data61.bin",
            "p0data62.bin",
            "p0data63.bin",
            "p0data7.bin"
        };

        public const String DESCRIPTION_FILE = "ModDescription.xml";
        public const String MOD_CONTENT_FILE = "ModFileList.txt";
        public const String INSTALLATION_TMP = "MemoriaInstallTmp";
    }
}
