using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Melt, Blowup
    /// </summary>
    [BattleScript(Id)]
    public sealed class MeltScript : IBattleScript
    {
        public const Int32 Id = 0088;

        private readonly BattleCalculator _v;

        public MeltScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            if (_v.Caster.CurrentHp != 0)
            {
                _v.Caster.Fig = (Int16)_v.Caster.CurrentHp;
                _v.Caster.Kill();
            }

            _v.Target.HpDamage = Math.Min((Int16)9999, _v.Caster.Fig);
        }
    }
}