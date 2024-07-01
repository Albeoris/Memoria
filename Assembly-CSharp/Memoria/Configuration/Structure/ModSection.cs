﻿using Memoria.Prime.Ini;
using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ModSection : IniSection
        {
            public readonly IniArray<String> FolderNames;
            public readonly IniArray<String> Priorities;
            public readonly IniValue<Int32> UseFileList;
            public readonly IniValue<Boolean> MergeScripts;

            public ModSection() : base(nameof(ModSection), true)
            {
                FolderNames = BindStringArray(nameof(FolderNames), new String[0]);
                Priorities = BindStringArray(nameof(Priorities), new String[0]);
                UseFileList = BindInt32(nameof(UseFileList), 1);
                MergeScripts = BindBoolean(nameof(MergeScripts), false);
            }
        }
    }
}
