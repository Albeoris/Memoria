using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Mod
        {
            public static String[] FolderNames => Instance._mod.FolderNames;
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