using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// 1,000 Needles, Pyro, Medeo, Poly
    /// </summary>
    [BattleScript(Id)]
    public sealed class ThousandNeedlesScript : IBattleScript
    {
        public const Int32 Id = 0026;

        private readonly BattleCalculator _v;

        public ThousandNeedlesScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = (Int16)(_v.Command.Power * 100 + _v.Command.HitRate);
        }
    }
}