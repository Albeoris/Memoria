using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Dead Pepper
    /// </summary>
    [BattleScript(Id)]
    public sealed class DeadPepperScript : IBattleScript
    {
        public const Int32 Id = 0075;

        private readonly BattleCalculator _v;

        public DeadPepperScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            Byte itemId = (Byte)_v.Command.AbilityId;

            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = (Int16)(_v.Command.Item.Power * (ff9item.FF9Item_GetCount(itemId) + 1));
        }
    }
}