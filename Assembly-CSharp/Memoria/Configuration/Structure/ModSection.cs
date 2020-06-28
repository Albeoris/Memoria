using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ModSection : IniSection
        {
            public readonly IniArray<String> FolderNames;

            public ModSection() : base(nameof(ModSection), true)
            {
                FolderNames = BindStringArray(nameof(FolderNames), new String[0]);
            }
        }
    }
}