using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Magic Hammer, Flare Star
    /// </summary>
    [BattleScript(Id)]
    public sealed class RandomMpDamageScript : IBattleScript
    {
        public const Int32 Id = 0031;

        private readonly BattleCalculator _v;

        public RandomMpDamageScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.MpAlteration;
            if (_v.Target.CurrentMp == 0)
                return;

            _v.Target.MpDamage = (Int16)Math.Min(9999, GameRandom.Next16() % _v.Target.CurrentMp);
        }
    }
}