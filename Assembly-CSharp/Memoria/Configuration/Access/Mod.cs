using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Mod
        {
            public static String[] FolderNames => Instance._mod.FolderNames;
        }
    }
}