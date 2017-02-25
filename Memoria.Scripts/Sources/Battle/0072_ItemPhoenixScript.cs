using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    ///  Phoenix Down, Phoenix Pinion
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemPhoenixScript : IBattleScript
    {
        public const Int32 Id = 0072;

        private readonly BattleCalculator _v;

        public ItemPhoenixScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CanBeRevived())
                return;
            
            if (_v.Target.IsZombie)
            {
                if ((_v.Target.CurrentHp = (UInt16)(GameRandom.Next8() % 10)) == 0)
                    _v.Target.Kill();
            }
            else if (_v.Target.CheckIsPlayer())
            {
                if (_v.Target.IsUnderStatus(BattleStatus.Death))
                    _v.Target.CurrentHp = (UInt16)(1 + GameRandom.Next8() % 10);

                _v.TargetCommand.TryRemoveItemStatuses();
            }
        }
    }
}