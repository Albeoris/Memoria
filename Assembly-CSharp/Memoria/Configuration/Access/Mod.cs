using System;
using System.Linq;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Mod
        {
            public static String[] FolderNames => Instance._mod.FolderNames;
            public static String[] Priorities => Instance._mod.Priorities;
            public static Int32 UseFileList => Instance._mod.UseFileList;
            public static Boolean MergeScripts => Instance._mod.MergeScripts;
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

            private static Boolean? _tranceSeek = null;
            public static Boolean TranceSeek
            {
                get
                {
                    if (!_tranceSeek.HasValue)
                        _tranceSeek = FolderNames.Contains("TranceSeek");
                    return _tranceSeek.Value;
                }
            }
        }
    }
}
