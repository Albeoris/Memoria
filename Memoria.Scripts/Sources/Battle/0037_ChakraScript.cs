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

            if (_v.Caster.HasSupportAbility(SupportAbility1.PowerUp))
            {
                _v.Target.HpDamage = (Int16)(_v.Target.MaximumHp * _v.Command.Power / 50);
                _v.Target.MpDamage = (Int16)(_v.Target.MaximumHp * _v.Command.Power / 50);
            }
            else
            {
                _v.Target.HpDamage = (Int16)(_v.Target.MaximumHp * _v.Command.Power / 100);
                _v.Target.MpDamage = (Int16)(_v.Target.MaximumHp * _v.Command.Power / 100);
            }
        }
    }
}