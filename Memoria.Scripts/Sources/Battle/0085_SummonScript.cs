using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Shiva, Ifrit, Ramuh, Leviathan, Bahamut, Ark, Fenrir, Madeen, Terra Homing
    /// </summary>
    [BattleScript(Id)]
    public sealed class SummonScript : IBattleScript
    {
        public const Int32 Id = 0085;

        private readonly BattleCalculator _v;

        public SummonScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalMagicParams();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.CasterCommand.BonusElement();
            if (!_v.CanAttackMagic())
                return;

            switch (_v.Command.AbilityId)
            {
                case BattleAbilityId.Shiva:
                    _v.Context.AttackPower += ff9item.FF9Item_GetCount(RegularItem.Opal);
                    break;
                case BattleAbilityId.Ifrit:
                    _v.Context.AttackPower += ff9item.FF9Item_GetCount(RegularItem.Topaz);
                    break;
                case BattleAbilityId.Ramuh:
                    _v.Context.AttackPower += ff9item.FF9Item_GetCount(RegularItem.Peridot);
                    break;
                case BattleAbilityId.Leviathan:
                    _v.Context.AttackPower += ff9item.FF9Item_GetCount(RegularItem.Aquamarine);
                    break;
                case BattleAbilityId.Bahamut:
                    _v.Context.AttackPower += ff9item.FF9Item_GetCount(RegularItem.Garnet);
                    break;
                case BattleAbilityId.Ark:
                    _v.Context.AttackPower += ff9item.FF9Item_GetCount(RegularItem.LapisLazuli);
                    break;
                case BattleAbilityId.Fenrir1:
                case BattleAbilityId.Fenrir2:
                    _v.Context.AttackPower += ff9item.FF9Item_GetCount(RegularItem.Sapphire);
                    break;
                case BattleAbilityId.Madeen:
                    _v.Context.AttackPower += _v.Caster.Level;
                    break;
            }

            _v.CalcHpDamage();
            _v.TryAlterMagicStatuses();
        }
    }
}