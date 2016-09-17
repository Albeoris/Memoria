using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Aerial Slash, Whirlwind, Flame Slash, Fire Blades, Jet Fire, Virus Crunch, Psychokinesis, Curse, Sandstorm, High Wind, Virus Fly, !!!, Leaf Swirl, Sweep, Fin, Boomerang, Paper Storm, Spin, Shockwave, Cleave, Raining Swords, Neutron Ring
    /// </summary>
    [BattleScript(Id)]
    public sealed class PreciseEnemyPhisicalAttackScript : IBattleScript
    {
        public const Int32 Id = 0100;

        private readonly BattleCalculator _v;

        public PreciseEnemyPhisicalAttackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.TryKillFrozen())
                return;

            _v.NormalPhisicalParams();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.GambleDefence();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.BonusBackstabAndPenaltyLongDistance();
            _v.CasterCommand.BonusElement();
            if (!_v.CanAttackElementalCommand())
                return;

            _v.TargetCommand.CalcHpDamage();
            _v.TargetCommand.TryAlterMagicStatuses();
        }
    }
}