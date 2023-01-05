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

            RegularItem itemId = (RegularItem)_v.Command.Power;
            MutableBattleCommand itemCommand = new MutableBattleCommand(_v.Caster, _v.Target.Id, itemId);
            SBattleCalculator.CalcMain(_v.Caster, _v.Target, itemCommand);
            BattleItem.RemoveFromInventory(itemId);
            _v.PerformCalcResult = false;
        }
    }
}