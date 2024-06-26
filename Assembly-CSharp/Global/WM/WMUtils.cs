using System;
using System.IO;

public static class WMUtils
{
    public static String EnsurePath(String path)
    {
        String directoryName = Path.GetDirectoryName(path);
        if (directoryName == null)
        {
            Debug.Log(path + " is not a valid path. Can't get a directory name");
            return String.Empty;
        }
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        return path;
    }

    public static void PauseEditor()
    {
    }
}
