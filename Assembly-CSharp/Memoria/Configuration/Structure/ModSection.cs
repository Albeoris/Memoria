using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ModSection : IniSection
        {
            public readonly IniArray<String> FolderNames;
            public readonly IniArray<String> Priorities;
            public readonly IniValue<Int32> GenerateFileList;
            public readonly IniValue<Boolean> TranceSeek;

            public ModSection() : base(nameof(ModSection), true)
            {
                FolderNames = BindStringArray(nameof(FolderNames), new String[0]);
                Priorities = BindStringArray(nameof(Priorities), new String[0]);
                GenerateFileList = BindInt32(nameof(GenerateFileList), 1);
                TranceSeek = BindBoolean(nameof(TranceSeek), false);
            }
        }
    }
}