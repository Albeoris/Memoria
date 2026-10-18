using System;
using System.Collections.Generic;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModValidationReport
    {
        public ModValidationReport(String rootName, IEnumerable<ModValidationResult> results)
        {
            Tree = ModValidationTreeNode.Create(rootName, results);
        }

        public ModValidationTreeNode Tree { get; }
        public IReadOnlyList<ModValidationResult> FlattenFiles() => Tree.FlattenFiles();
    }
}