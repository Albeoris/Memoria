using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Mod
        {
            public static String[] FolderNames => Instance._mod.FolderNames;
            public static String[] Priorities => Instance._mod.Priorities;
            public static Int32 GenerateFileList => Instance._mod.GenerateFileList;
            public static Boolean TranceSeek => Instance._mod.TranceSeek;
            public static String[] AllFolderNames
            {
                get
                {
                    String[] res = new String[FolderNames.Length + 1];
                    FolderNames.CopyTo(res, 0);
                    res[FolderNames.Length] = "";
                    return res;
                }
            }
        }
    }
}