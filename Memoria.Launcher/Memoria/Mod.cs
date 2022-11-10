using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace Memoria.Launcher
{
	public class Mod
    {
        public String Name { get; set; }
        public Version CurrentVersion { get; set; }
        public String ReleaseDate { get; set; }
        public String Author { get; set; }
        public String Description { get; set; }
        public String PatchNotes { get; set; }
        public String Category { get; set; }
        public String Website { get; set; }
        public String DescriptionUrl { get; set; } // TODO (fetch ModDescription.xml online)
        public String DownloadUrl { get; set; }
        public String InstallationPath { get; set; }
        public String DownloadFormat { get; set; }
        public String PreviewFile { get; set; }
        public String PreviewFileUrl { get; set; }
        public List<String> FileContent { get; set; } // TODO (give informations about mod compatibilities)
        public Int64 DownloadSize { get; set; } // TODO
        public Int64 FullSize { get; set; } // TODO
        public List<Mod> SubMod { get; set; }
        public Boolean IsActive { get; set; }
        public String Installed { get; set; }
        public String DownloadSpeed { get; set; }
        public String RemainingTime { get; set; }
        public Int64 PercentComplete { get; set; }
        public Int32 Priority { get; set; }
        public BitmapImage PreviewImage { get; set; }

        public Mod()
        {
            FileContent = new List<String>();
            SubMod = new List<Mod>();
        }

        public Mod(String name, String path)
        {
            Name = name;
            InstallationPath = path;
            FileContent = new List<String>();
            SubMod = new List<Mod>();
        }

        public Mod(StreamReader reader)
        {
            FileContent = new List<String>();
            SubMod = new List<Mod>();
            ReadDescription(reader);
        }

        public Mod(String folderPath)
        {
            FileContent = new List<String>();
            SubMod = new List<Mod>();
            try
            {
                using (Stream input = File.OpenRead(folderPath + "/" + DESCRIPTION_FILE))
                using (StreamReader reader = new StreamReader(input))
                    ReadDescription(reader);
                if (InstallationPath == null)
                    InstallationPath = folderPath;
            }
            catch (Exception err)
			{
			}
        }

        public Boolean ReadDescription(StreamReader reader)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            XmlNodeList modList = doc.SelectNodes("/Mod");
            if (modList.Count != 1)
                return false;
            XmlElement elName = modList[0]["Name"];
            XmlElement elInstPath = modList[0]["InstallationPath"];
            if (elName == null || elInstPath == null)
                return false;
            XmlElement elVer = modList[0]["Version"];
            XmlElement elRelease = modList[0]["ReleaseDate"];
            XmlElement elAuthor = modList[0]["Author"];
            XmlElement elDescription = modList[0]["Description"];
            XmlElement elPatchNotes = modList[0]["PatchNotes"];
            XmlElement elCategory = modList[0]["Category"];
            XmlElement elWebsite = modList[0]["Website"];
            XmlElement elDL = modList[0]["DownloadUrl"];
            XmlElement elFormat = modList[0]["DownloadFormat"];
            XmlElement elFullSize = modList[0]["FullSize"];
            XmlElement elDLSize = modList[0]["DownloadSize"];
            XmlElement elPreviewFile = modList[0]["PreviewFile"];
            XmlElement elPreviewFileUrl = modList[0]["PreviewFileUrl"];
            XmlNodeList elSubMod = modList[0].SelectNodes("SubMod");
            Int64 outParse;
            Name = elName.InnerText;
            CurrentVersion = elVer != null ? new Version(elVer.InnerText) : null;
            InstallationPath = elInstPath.InnerText;
            ReleaseDate = elRelease?.InnerText;
            Author = elAuthor?.InnerText;
            Description = elDescription?.InnerText;
            PatchNotes = elPatchNotes?.InnerText;
            Category = elCategory?.InnerText;
            Website = elWebsite?.InnerText;
            DownloadUrl = elDL?.InnerText;
            DownloadFormat = elFormat?.InnerText;
            PreviewFile = elPreviewFile?.InnerText;
            PreviewFileUrl = elPreviewFileUrl?.InnerText;
            if (Int64.TryParse(elFullSize?.InnerText ?? "0", out outParse)) FullSize = outParse;
            if (Int64.TryParse(elDLSize?.InnerText ?? "0", out outParse)) DownloadSize = outParse;
            SubMod.Clear();
            foreach (XmlNode subNode in elSubMod)
			{
                Mod sub = new Mod();
                elName = subNode["Name"];
                elInstPath = subNode["InstallationPath"];
                if (elName == null || elInstPath == null)
                    continue;
                XmlElement elPriority = subNode["Priority"];
                elDescription = subNode["Description"];
                elCategory = subNode["Category"];
                sub.Name = elName.InnerText;
                sub.InstallationPath = elInstPath.InnerText;
                sub.Description = elDescription?.InnerText;
                sub.Category = elCategory?.InnerText;
                if (Int64.TryParse(elPriority?.InnerText ?? "0", out outParse))
                    sub.Priority = (Int32)outParse;
                else
                    sub.Priority = 0;
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

        public static Mod SearchWithName(IEnumerable<Mod> modList, String queryName)
		{
            foreach (Mod mod in modList)
                if (mod.Name == queryName)
                    return mod;
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

        public const String DESCRIPTION_FILE = "ModDescription.xml";
        public const String INSTALLATION_TMP = "MemoriaInstallTmp";
    }
}
