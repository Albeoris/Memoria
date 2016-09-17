using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Regen, Shell, Protect, Haste, Reflect, Float, Carbuncle, Mighty Guard, Vanish, Auto-Life, Reis’s Wind, Luna, Aura, Defend
    /// </summary>
    [BattleScript(Id)]
    public sealed class MagicApplyPositiveStatusScript : IBattleScript
    {
        public const Int32 Id = 0103;

        private readonly BattleCalculator _v;

        public MagicApplyPositiveStatusScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.TargetCommand.TryAlterCommandStatuses();
        }
    }
}