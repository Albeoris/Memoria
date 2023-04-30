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
            RegularItem weaponNumber = _v.Caster.Weapon;
            switch (weaponNumber)
            {
                case RegularItem.ButterflySword:
                case RegularItem.TheOgre:
                case RegularItem.Exploda:
                case RegularItem.RuneTooth:
                case RegularItem.AngelBless:
                case RegularItem.Sargatanas:
                case RegularItem.Masamune:
                case RegularItem.TheTower:
                case RegularItem.UltimaWeapon:
                    break;
                default:
                    _v.Context.Flags |= BattleCalcFlags.Miss;
                    return;
            }

            BattleStatus status = _v.Caster.WeaponStatus;
            if ((status & BattleStatus.Death) == 0 || _v.Target.CheckUnsafetyOrGuard())
                _v.Target.TryAlterStatuses(status, true, _v.Caster);
        }
    }
}