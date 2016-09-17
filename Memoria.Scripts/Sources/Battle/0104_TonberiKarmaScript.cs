using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Everyone's Grudge (Karma)
    /// </summary>
    [BattleScript(Id)]
    public sealed class TonberiKarmaScript : IBattleScript
    {
        public const Int32 Id = 0104;

        private readonly BattleCalculator _v;

        public TonberiKarmaScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = (Int16)(GameState.Tonberies > 13 ? 9999 : 1 << GameState.Tonberies - 1);
        }
    }
}