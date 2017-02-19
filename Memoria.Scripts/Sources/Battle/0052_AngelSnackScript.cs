using System;
using Memoria.Data;

// ReSharper disable UseObjectOrCollectionInitializer

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Angel’s Snack
    /// </summary>
    [BattleScript(Id)]
    public sealed class AngelSnackScript : IBattleScript
    {
        public const Int32 Id = 0052;

        private readonly BattleCalculator _v;

        public AngelSnackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.CheckHasCommandItem())
                return;

            Byte itemId = _v.Command.Power;
            BattleItem item = BattleItem.Find(itemId);
            Byte itemScript = item.ScriptId;

            MutableBattleCommand itemCommand = new MutableBattleCommand();
            itemCommand.Id = BattleCommandId.Item;
            itemCommand.AbilityId = BattleAbilityId.Void;
            itemCommand.LoadAbility();
            itemCommand.AbilityId = (BattleAbilityId)itemId;

            SBattleCalculator.Calc(_v.Caster, _v.Target, itemCommand, itemScript);
            BattleItem.RemoveFromInventory(itemId);
        }
    }
}