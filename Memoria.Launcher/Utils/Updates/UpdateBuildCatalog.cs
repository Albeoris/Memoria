using System;
using System.Collections.Generic;

namespace Memoria.Launcher.Utils.Updates
{
    internal static class UpdateBuildCatalog
    {
        public static IReadOnlyList<UpdateBuild> All { get; } = new[]
        {
            new UpdateBuild(UpdateBuildKind.Stable, new Uri("https://github.com/Albeoris/Memoria/releases/latest/download/Memoria.Patcher.exe")),
            new UpdateBuild(UpdateBuildKind.Canary, new Uri("https://github.com/Albeoris/Memoria/releases/download/canary/Memoria.Patcher.exe"))
        };
    }
}
