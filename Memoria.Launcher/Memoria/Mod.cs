using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        public List<String> FileContent { get; set; } // TODO (give informations about mod compatibilities)
        public Int64 DownloadSize { get; set; } // TODO
        public Int64 FullSize { get; set; } // TODO
        public Boolean IsActive { get; set; }
        public String Installed { get; set; }
        public String DownloadSpeed { get; set; }
        public String RemainingTime { get; set; }
        public Int64 PercentComplete { get; set; }
        public Int32 Priority { get; set; }

        public Mod()
        {
            FileContent = new List<String>();
        }

        public Mod(String name, String path)
        {
            Name = name;
            InstallationPath = path;
            FileContent = new List<String>();
        }

        public Mod(StreamReader reader)
        {
            FileContent = new List<String>();
            ReadDescription(reader);
        }

        public Mod(String folderPath)
        {
            FileContent = new List<String>();
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
            if (elName == null)
                return false;
            XmlElement elVer = modList[0]["Version"];
            XmlElement elInstPath = modList[0]["InstallationPath"];
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
            Int64 outParse;
            Name = elName.InnerText;
            CurrentVersion = elVer != null ? new Version(elVer.InnerText) : null;
            InstallationPath = elInstPath?.InnerText;
            ReleaseDate = elRelease?.InnerText;
            Author = elAuthor?.InnerText;
            Description = elDescription?.InnerText;
            PatchNotes = elPatchNotes?.InnerText;
            Category = elCategory?.InnerText;
            Website = elWebsite?.InnerText;
            DownloadUrl = elDL?.InnerText;
            DownloadFormat = elFormat?.InnerText;
            if (Int64.TryParse(elFullSize?.InnerText ?? "0", out outParse)) FullSize = outParse;
            if (Int64.TryParse(elDLSize?.InnerText ?? "0", out outParse)) DownloadSize = outParse;
            return true;
        }

        public void GenerateDescription(String folderPath)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mod = doc.CreateNode(XmlNodeType.Element, "Mod", "");
            doc.AppendChild(mod);
            XmlElement el = doc.CreateElement("Name");
            el.InnerXml = Name;
            mod.AppendChild(el);
            if (CurrentVersion != null)
            {
                el = doc.CreateElement("Version");
                el.InnerXml = CurrentVersion.ToString();
                mod.AppendChild(el);
            }
            if (InstallationPath != null)
            {
                el = doc.CreateElement("InstallationPath");
                el.InnerXml = InstallationPath;
                mod.AppendChild(el);
            }
            if (ReleaseDate != null)
            {
                el = doc.CreateElement("ReleaseDate");
                el.InnerXml = ReleaseDate;
                mod.AppendChild(el);
            }
            if (Author != null)
            {
                el = doc.CreateElement("Author");
                el.InnerXml = Author;
                mod.AppendChild(el);
            }
            if (Description != null)
            {
                el = doc.CreateElement("Description");
                el.InnerXml = Description;
                mod.AppendChild(el);
            }
            if (PatchNotes != null)
            {
                el = doc.CreateElement("PatchNotes");
                el.InnerXml = PatchNotes;
                mod.AppendChild(el);
            }
            if (Category != null)
            {
                el = doc.CreateElement("Category");
                el.InnerXml = Category;
                mod.AppendChild(el);
            }
            if (Website != null)
            {
                el = doc.CreateElement("Website");
                el.InnerXml = Website;
                mod.AppendChild(el);
            }
            if (DownloadUrl != null)
            {
                el = doc.CreateElement("DownloadUrl");
                el.InnerXml = DownloadUrl;
                mod.AppendChild(el);
            }
            if (DownloadFormat != null)
            {
                el = doc.CreateElement("DownloadFormat");
                el.InnerXml = DownloadFormat;
                mod.AppendChild(el);
            }
            if (FullSize > 0)
            {
                el = doc.CreateElement("FullSize");
                el.InnerXml = FullSize.ToString();
                mod.AppendChild(el);
            }
            if (DownloadSize > 0)
            {
                el = doc.CreateElement("DownloadSize");
                el.InnerXml = DownloadSize.ToString();
                mod.AppendChild(el);
            }
            File.WriteAllText(folderPath + "/" + DESCRIPTION_FILE, Regex.Replace(doc.OuterXml, @">(<[^/])", ">\n\t$1").Replace("</Mod>", "\n</Mod>"));
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
