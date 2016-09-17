using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Dark Matter
    /// </summary>
    [BattleScript(Id)]
    public sealed class DarkMatterScript : IBattleScript
    {
        public const Int32 Id = 0077;

        private readonly BattleCalculator _v;

        public DarkMatterScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.CanBeAttacked())
            {
                _v.Target.Flags |= CalcFlag.HpAlteration;
                _v.Target.HpDamage = 9999;
            }
        }
    }
}