using System;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class DexterityWeaponScript : BaseWeaponScript
    {
        public const Int32 Id = 0003;

        public DexterityWeaponScript(BattleCalculator v)
            : base(v, CalcAttackBonus.Dexterity)
        {
        }
    }
}