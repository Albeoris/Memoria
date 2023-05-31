using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Lancer
    /// </summary>
    [BattleScript(Id)]
    public sealed class LancerScript : IBattleScript
    {
        public const Int32 Id = 0039;

        private readonly BattleCalculator _v;

        public LancerScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.WeaponPhysicalParams();
            _v.Caster.EnemyTranceBonusAttack();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.CalcHpDamage();
            _v.Target.Flags |= CalcFlag.MpAlteration;
            if ((_v.Target.Flags & CalcFlag.HpRecovery) != 0)
                _v.Target.Flags |= CalcFlag.MpRecovery;
            _v.Target.MpDamage = _v.Target.HpDamage >> 4;
        }
    }
}