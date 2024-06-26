using Memoria.Data;
using System;

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
            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            if (_v.TryMagicHit())
                BattleItem.AddToInventory(RegularItem.Ore);
        }
    }
}
