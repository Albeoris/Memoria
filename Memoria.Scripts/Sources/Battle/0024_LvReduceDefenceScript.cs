using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// LV3 Def-less
    /// </summary>
    [BattleScript(Id)]
    public sealed class LvReduceDefenceScript : IBattleScript
    {
        public const Int32 Id = 0024;

        private readonly BattleCalculator _v;

        public LvReduceDefenceScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.IsTargetLevelMultipleOfCommandRate() && _v.Target.CanBeAttacked())
            {
                if (_v.Target.PhysicalDefence != 0)
                    _v.Target.PhysicalDefence = GameRandom.Next16() % _v.Target.PhysicalDefence;
                if (_v.Target.MagicDefence != 0)
                    _v.Target.MagicDefence = GameRandom.Next16() % _v.Target.MagicDefence;
            }
        }
    }
}
