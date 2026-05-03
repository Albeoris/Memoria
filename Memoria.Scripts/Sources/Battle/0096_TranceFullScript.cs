using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Trance Full, Transfer
    /// </summary>
    [BattleScript(Id)]
    public sealed class TranceFullScript : IBattleScript
    {
        public const Int32 Id = 0096;

        private readonly BattleCalculator _v;

        public TranceFullScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.HasTrance)
            {
                _v.Context.Flags |= BattleCalcFlags.Miss;
                return;
            }
            _v.Target.Trance = Byte.MaxValue;
            _v.Target.AlterStatus(BattleStatusId.Trance);
        }
    }
}
