using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// LV3 Def-less
    /// </summary>
    [BattleScript(Id)]
    public sealed class ReduceDefenceScript : IBattleScript
    {
        public const Int32 Id = 0024;

        private readonly BattleCalculator _v;

        public ReduceDefenceScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.TargetCommand.IsTargetLevelMultipleOfCommandRate() && _v.Target.CanBeAttacked())
            {
                if (_v.Target.PhisicalDefence != 0)
                    _v.Target.PhisicalDefence = (Byte)((UInt32)GameRandom.Next16() % _v.Target.PhisicalDefence);
                if (_v.Target.MagicDefence != 0)
                    _v.Target.MagicDefence = (Byte)((UInt32)GameRandom.Next16() % _v.Target.MagicDefence);
            }
        }
    }
}