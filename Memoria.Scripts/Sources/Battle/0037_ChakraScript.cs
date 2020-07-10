using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Chakra
    /// </summary>
    [BattleScript(Id)]
    public sealed class ChakraScript : IBattleScript
    {
        public const Int32 Id = 0037;

        private readonly BattleCalculator _v;

        public ChakraScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= (CalcFlag)27;

            _v.Target.HpDamage = (Int32)(_v.Target.MaximumHp * _v.Command.Power / 100);
            _v.Target.MpDamage = (Int32)(_v.Target.MaximumMp * _v.Command.Power / 100);
        }
    }
}
