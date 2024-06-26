using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Everyone's Grudge (Karma)
    /// </summary>
    [BattleScript(Id)]
    public sealed class TonberryKarmaScript : IBattleScript
    {
        public const Int32 Id = 0104;

        private readonly BattleCalculator _v;

        public TonberryKarmaScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = 1 << Math.Min(20, GameState.Tonberies - 1);
        }
    }
}