using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Absorb Strength
    /// </summary>
    [BattleScript(Id)]
    public sealed class AbsorbStrengthScript : IBattleScript
    {
        public const Int32 Id = 0095;

        private readonly BattleCalculator _v;

        public AbsorbStrengthScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Strength /= 2;
            _v.Caster.Strength = (Byte)Math.Min(Byte.MaxValue, _v.Caster.Strength * 2);
        }
    }
}