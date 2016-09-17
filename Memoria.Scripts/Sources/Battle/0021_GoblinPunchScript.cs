using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Goblin Punch
    /// </summary>
    [BattleScript(Id)]
    public sealed class GoblinPunchScript : IBattleScript
    {
        public const Int32 Id = 0021;

        private readonly BattleCalculator _v;

        public GoblinPunchScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalMagicParams();
            if (_v.Target.Level == _v.Caster.Level)
            {
                _v.Context.Attack += _v.Caster.Level;
                _v.Context.DefensePower = 0;
            }
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.TargetCommand.CalcHpDamage();
        }
    }
}