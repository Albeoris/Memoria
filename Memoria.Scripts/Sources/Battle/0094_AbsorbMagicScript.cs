using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Absorb Magic
    /// </summary>
    [BattleScript(Id)]
    public sealed class AbsorbMagicScript : IBattleScript
    {
        public const Int32 Id = 0094;

        private readonly BattleCalculator _v;

        public AbsorbMagicScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Magic /= 2;
            _v.Caster.Magic = (Byte)Math.Min(Byte.MaxValue, _v.Caster.Magic * 2);
        }
    }
}