using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Panacea, Stona, Esuna, Dispel
    /// </summary>
    [BattleScript(Id)]
    public sealed class MagicCureStatusScript : IBattleScript
    {
        public const Int32 Id = 0012;

        private readonly BattleCalculator _v;

        public MagicCureStatusScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.TargetCommand.TryRemoveAbilityStatuses();
        }
    }
}