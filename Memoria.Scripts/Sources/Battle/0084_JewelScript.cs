using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Jewel
    /// Extracts Ore from a target.
    /// </summary>
    [BattleScript(Id)]
    public sealed class JewelScript : IBattleScript
    {
        public const Int32 Id = 0084;

        private readonly BattleCalculator _v;

        public JewelScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            const Int32 oreItemId = (Int32)GemItem.Ore;

            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            if (_v.TargetCommand.TryMagicHit())
                BattleItem.AddToInventory(oreItemId);
        }
    }
}