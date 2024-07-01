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
            _v.Target.HpDamage = (Int32)_v.Caster.CurrentHp;

            if (_v.Caster.CurrentHp != 0)
            {
                _v.Caster.AddDelayedModifier(
                    null,
                    caster =>
                    {
                        if (!BattleState.IsBattleStateEnabled || caster.CurrentHp == 0)
                            return;
                        caster.Kill(caster);
                    }
                );
            }
        }
    }
}
