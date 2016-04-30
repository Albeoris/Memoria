using System;
using System.Collections;
using System.IO;

namespace Memoria
{
    public abstract class TextImporter
    {
        public IEnumerator LoadAsync()
        {
            Task<Boolean> task;
            if (Configuration.Import.Enabled && Configuration.Import.Text)
            {
                task = Task.Run(LoadExternal);
                while (!task.IsCompleted)
                    yield return 0;

                if (task.State == TaskState.Success && task.Result)
                    yield break;
            }

            task = Task.Run(LoadInternal);
            while (!task.IsCompleted)
                yield return 0;

            if (!task.Result)
                throw new Exception("Failed to load embaded resources.");
        }

        protected abstract Boolean LoadExternal();
        protected abstract Boolean LoadInternal();
    }

    public abstract class SingleFileImporter : TextImporter
    {
        protected abstract String TypeName { get; }
        protected abstract String ImportPath { get; }
        protected abstract void ProcessEntries(TxtEntry[] entreis);

        protected override Boolean LoadExternal()
        {
            try
            {
                String importPath = ImportPath;
                if (!File.Exists(importPath))
                {
                    Log.Warning($"[{TypeName}] Import was skipped bacause file does not exist: [{importPath}].");
                    return false;
                }

                Log.Message($"[{TypeName}] Exporting...");

                TxtEntry[] entries = TxtReader.ReadStrings(importPath);

                ProcessEntries(entries);

                Log.Message($"[{TypeName}] Importing completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to import resource.");
                return false;
            }
        }
    }

    public class AbilityImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(AbilityImporter);
        protected override string ImportPath => ModTextResources.Import.Abilities;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] skillNames, skillHelps;
            AbilityFormatter.Parse(entreis, out skillNames, out skillHelps);

            FF9TextToolAccessor.SetSupportAbilityName(skillNames);
            FF9TextToolAccessor.SetSupportAbilityHelpDesc(skillHelps);
        }

        protected override Boolean LoadInternal()
        {
            String[] skillNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillNames);
            String[] skillHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillHelps);

            FF9TextToolAccessor.SetSupportAbilityName(skillNames);
            FF9TextToolAccessor.SetSupportAbilityHelpDesc(skillHelps);
            return true;
        }
    }
}