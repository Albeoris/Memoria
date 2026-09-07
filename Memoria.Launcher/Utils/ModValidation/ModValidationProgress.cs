using System;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModValidationProgress
    {
        public ModValidationProgress(String currentFile, Int32 discoveredFiles, Int32 completedFiles, Boolean enumerationCompleted, ModValidationResult completedResult)
        {
            CurrentFile = currentFile ?? String.Empty;
            DiscoveredFiles = discoveredFiles;
            CompletedFiles = completedFiles;
            EnumerationCompleted = enumerationCompleted;
            CompletedResult = completedResult;
        }

        public String CurrentFile { get; }
        public Int32 DiscoveredFiles { get; }
        public Int32 CompletedFiles { get; }
        public Boolean EnumerationCompleted { get; }
        public ModValidationResult CompletedResult { get; }
    }
}