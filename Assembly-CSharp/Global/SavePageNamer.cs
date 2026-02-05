using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SavePageNamer
{
    private static Dictionary<int, string> pageNames = new Dictionary<int, string>();
    private static string filePath;

    static SavePageNamer()
    {
        if (string.IsNullOrEmpty(SharedDataBytesStorage.MetaData.DirPath))
        {
            SharedDataBytesStorage.SetPCPath();
        }

        string saveFolder = SharedDataBytesStorage.MetaData.DirPath;

        if (string.IsNullOrEmpty(saveFolder))
        {
            saveFolder = Application.persistentDataPath + "/EncryptedSavedData";
        }

        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        filePath = Path.Combine(saveFolder, "ExtraSave.ini");
        Load();
    }

    public static void Load()
    {
        pageNames.Clear();
        if (!File.Exists(filePath)) return;

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line) || !line.Contains("=")) continue;
            string[] parts = line.Split(new char[] { '=' }, 2);
            if (int.TryParse(parts[0], out int pageId))
            {
                pageNames[pageId] = parts.Length > 1 ? parts[1] : "";
            }
        }
    }

    public static void Save()
    {
        List<string> lines = new List<string>();
        foreach (var kvp in pageNames)
        {
            lines.Add($"{kvp.Key}={kvp.Value}");
        }
        File.WriteAllLines(filePath, lines.ToArray());
    }

    public static string GetPageName(int pageIndex)
    {
        if (pageNames.ContainsKey(pageIndex) && !string.IsNullOrEmpty(pageNames[pageIndex]))
        {
            return pageNames[pageIndex];
        }

        // Nom par défaut si rien n'est défini
        int startSlot = (pageIndex * 10) + 1;
        return $"Page {pageIndex + 1}";
    }

    public static void SetPageName(int pageIndex, string name)
    {
        pageNames[pageIndex] = name;
        Save();
    }
}
