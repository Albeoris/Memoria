using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Pumpkin Head, Minus Strike, Chestnut
    /// </summary>
    [BattleScript(Id)]
    public sealed class DifferentCasterHpAttackScript : IBattleScript
    {
        public const Int32 Id = 0029;

        private readonly BattleCalculator _v;

        public DifferentCasterHpAttackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = (Int16)(_v.Caster.MaximumHp - _v.Caster.CurrentHp);
        }
    }
}