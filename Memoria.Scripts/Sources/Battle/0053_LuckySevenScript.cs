using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Lucky Seven
    /// </summary>
    [BattleScript(Id)]
    public sealed class LuckySevenScript : IBattleScript
    {
        public const Int32 Id = 0053;

        private readonly BattleCalculator _v;

        public LuckySevenScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            if (_v.Caster.CurrentHp % 10 != 7)
            {
                _v.Target.HpDamage = 1;
                return;
            }

            switch (GameRandom.Next8() % 4)
            {
                case 0:
                    _v.Target.HpDamage = 7;
                    break;
                case 1:
                    _v.Target.HpDamage = 77;
                    break;
                case 2:
                    _v.Target.HpDamage = 777;
                    break;
                default:
                    _v.Target.HpDamage = 7777;
                    break;
            }
        }
    }
}