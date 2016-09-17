using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Maelstrom
    /// </summary>
    [BattleScript(Id)]
    public sealed class MaelstromScript : IBattleScript
    {
        public const Int32 Id = 0093;

        private readonly BattleCalculator _v;

        public MaelstromScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.CheckUnsafetyOrMiss() && _v.Target.CanBeAttacked())
            {
                _v.Context.Flags |= BattleCalcFlags.DirectHP;
                _v.Target.CurrentHp = (UInt16)(1 + GameRandom.Next8() % 9);
                _v.Target.TryAlterStatuses(_v.Command.AbilityStatus, false);
            }
        }
    }
}