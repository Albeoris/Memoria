using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Ether
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemEtherScript : IBattleScript
    {
        public const Int32 Id = 0070;

        private readonly BattleCalculator _v;

        public ItemEtherScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Context.Attack = 15;
            _v.Context.AttackPower = _v.Command.Item.Power;
            _v.Context.DefensePower = 0;

            if (_v.Caster.HasSupportAbility(SupportAbility1.Chemist))
                _v.Context.Attack *= 2;

            _v.Target.Flags |= CalcFlag.MpAlteration;
            if (!_v.Target.IsZombie)
                _v.Target.Flags |= CalcFlag.MpRecovery;

            _v.Target.MpDamage = (Int16)Math.Min(9999, _v.Context.AttackPower * _v.Context.Attack);
        }
    }
}