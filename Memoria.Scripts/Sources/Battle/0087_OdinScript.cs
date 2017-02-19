using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Odin
    /// </summary>
    [BattleScript(Id)]
    public sealed class OdinScript : IBattleScript
    {
        public const Int32 Id = 0087;

        private readonly BattleCalculator _v;

        public OdinScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.CheckUnsafetyOrGuard())
            {
                _v.MagicAccuracy();
                _v.Context.HitRate += (Int16)(ff9item.FF9Item_GetCount((Int16)GemItem.Ore) >> 1);
                if (_v.TargetCommand.TryMagicHit())
                    _v.TargetCommand.TryAlterCommandStatuses();
            }

            if ((_v.Context.Flags & (BattleCalcFlags.Guard | BattleCalcFlags.Miss)) != 0 && _v.Caster.HasSupportAbility(SupportAbility2.OdinSword))
            {
                _v.Context.Flags &= (BattleCalcFlags)65470;
                _v.Context.Attack = (Int16)(_v.Caster.Magic + GameRandom.Next16() % (1 + (_v.Caster.Level + _v.Caster.Magic >> 3)));
                _v.Context.AttackPower = (Int16)(_v.Command.Power + 100 - ff9item.FF9Item_GetCount((Int16)GemItem.Ore));
                _v.Context.DefensePower = _v.Target.MagicDefence;
                _v.TargetCommand.CalcHpDamage();
            }
        }
    }
}