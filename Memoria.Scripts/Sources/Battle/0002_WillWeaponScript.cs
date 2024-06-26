using System;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class WillWeaponScript : BaseWeaponScript
    {
        public const Int32 Id = 0002;

        public WillWeaponScript(BattleCalculator v)
            : base(v, CalcAttackBonus.WillPower)
        {
        }
    }
}