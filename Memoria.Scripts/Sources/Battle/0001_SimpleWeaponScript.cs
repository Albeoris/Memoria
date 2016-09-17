using System;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class SimpleWeaponScript : BaseWeaponScript
    {
        public const Int32 Id = 0001;

        public SimpleWeaponScript(BattleCalculator v)
            : base(v, CalcAttackBonus.Simple)
        {
        }
    }
}