using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Dead Pepper
    /// </summary>
    [BattleScript(Id)]
    public sealed class TentScript : IBattleScript
    {
        public const Int32 Id = 0076;

        private readonly BattleCalculator _v;

        public TentScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration | CalcFlag.HpRecovery | CalcFlag.MpAlteration | CalcFlag.MpRecovery;
            _v.Target.HpDamage = (Int16)(_v.Target.MaximumHp >> 1);
            _v.Target.MpDamage = (Int16)(_v.Target.MaximumMp >> 1);

            BattleItem item = _v.Command.Item;
            if (item.HitRate < GameRandom.Next16() % 100)
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.Bitten);
                _v.Target.TryAlterStatuses(item.Status, false);
            }
        }
    }
}