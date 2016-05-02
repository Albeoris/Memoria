using System;

namespace Memoria
{
    public class AbilityImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(AbilityImporter);
        protected override String ImportPath => ModTextResources.Import.Abilities;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] abilityNames, strings;
            AbilityFormatter.Parse(entreis, out abilityNames, out strings);

            FF9TextToolAccessor.SetSupportAbilityName(abilityNames);
            FF9TextToolAccessor.SetSupportAbilityHelpDesc(strings);
        }

        protected override Boolean LoadInternal()
        {
            String[] abilityNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityNames);
            String[] abilityHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityHelps);

            FF9TextToolAccessor.SetSupportAbilityName(abilityNames);
            FF9TextToolAccessor.SetSupportAbilityHelpDesc(abilityHelps);
            return true;
        }
    }
}