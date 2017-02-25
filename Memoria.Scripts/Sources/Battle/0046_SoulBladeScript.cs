using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Soul Blade
    /// </summary>
    [BattleScript(Id)]
    public sealed class SoulBladeScript : IBattleScript
    {
        public const Int32 Id = 0046;

        private readonly BattleCalculator _v;

        public SoulBladeScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            WeaponItem weaponNumber = _v.Caster.Weapon;
            switch (weaponNumber)
            {
                case WeaponItem.ButterflySword:
                case WeaponItem.TheOgre:
                case WeaponItem.Exploda:
                case WeaponItem.RuneTooth:
                case WeaponItem.AngelBless:
                case WeaponItem.Sargatanas:
                case WeaponItem.Masamune:
                case WeaponItem.TheTower:
                case WeaponItem.UltimaWeapon:
                    break;
                default:
                    _v.Context.Flags |= BattleCalcFlags.Miss;
                    return;
            }

            BattleStatus status = _v.Caster.WeaponStatus;
            if ((status & BattleStatus.Death) == 0 || _v.Target.CheckUnsafetyOrGuard())
                _v.Target.TryAlterStatuses(status, true);
        }
    }
}