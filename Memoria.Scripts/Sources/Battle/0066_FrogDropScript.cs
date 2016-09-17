using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Frog Drop
    /// </summary>
    [BattleScript(Id)]
    public sealed class FrogDropScript : IBattleScript
    {
        public const Int32 Id = 0066;

        private readonly BattleCalculator _v;

        public FrogDropScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;

            if (GameState.Frogs == 0)
                _v.Target.HpDamage = 1;
            else
                _v.Target.HpDamage = (Int16)Math.Min(9999, GameState.Frogs * _v.Caster.Level);
        }
    }
}