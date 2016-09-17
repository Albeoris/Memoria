using System;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class LevelWeaponScript : BaseWeaponScript
    {
        public const Int32 Id = 0007;

        public LevelWeaponScript(BattleCalculator v)
            : base(v, CalcAttackBonus.Level)
        {
        }
    }
}