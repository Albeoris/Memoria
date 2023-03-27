using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Double cast with the spells defined by "Power" and "Accuracy"
    /// This script is never executed: the effect is handled by btl_cmd
    /// </summary>
    [BattleScript(Id)]
    public sealed class DoubleCastSpecial : IBattleScript
    {
        public const Int32 Id = 0080;

        private readonly BattleCalculator _v;

        public DoubleCastSpecial(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
        }
    }
}