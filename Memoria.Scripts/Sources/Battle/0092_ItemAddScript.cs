using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// ???
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemAddScript : IBattleScript
    {
        public const Int32 Id = 0092;

        private readonly BattleCalculator _v;

        public ItemAddScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            Byte itemId = _v.Command.Power;
            BattleItem.AddToInventory(itemId);
        }
    }
}