using System;
using Memoria.Data;

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
            _v.Target.Trance = Byte.MaxValue;
            _v.Target.AlterStatus(BattleStatus.Trance);
        }
    }
}