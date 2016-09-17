using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Six Dragons
    /// </summary>
    [BattleScript(Id)]
    public sealed class SixDragonsScript : IBattleScript
    {
        public const Int32 Id = 0050;

        private readonly BattleCalculator _v;

        public SixDragonsScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CanBeAttacked())
                return;

            Int32 percent = GameRandom.Next16() % 100;

            if (percent < 10)
            {
                _v.Target.CurrentHp = _v.Target.MaximumHp;
                _v.Target.CurrentMp = _v.Target.MaximumMp;
                return;
            }

            if (percent < 30)
            {
                _v.Target.CurrentHp = _v.Target.MaximumHp;
                return;
            }

            if (percent < 50)
            {
                _v.Target.CurrentMp = _v.Target.MaximumMp;
                return;
            }

            if (percent < 65)
            {
                _v.Target.CurrentHp = 1;
                return;
            }

            if (percent < 80)
            {
                _v.Target.CurrentMp = 1;
                return;
            }

            _v.Target.CurrentHp = 1;
            _v.Target.CurrentMp = 1;
        }
    }
}