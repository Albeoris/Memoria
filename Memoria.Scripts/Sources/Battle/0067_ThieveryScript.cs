using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Thievery
    /// </summary>
    [BattleScript(Id)]
    public sealed class ThieveryScript : IBattleScript
    {
        public const Int32 Id = 0067;

        private readonly BattleCalculator _v;

        public ThieveryScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = (Int16)Math.Min(9999, GameState.Thefts * _v.Caster.Dexterity / 2);
        }
    }
}