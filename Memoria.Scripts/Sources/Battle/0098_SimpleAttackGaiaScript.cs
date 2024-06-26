using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// ???
    /// </summary>
    [BattleScript(Id)]
    public sealed class SimpleAttackGaiaScript : IBattleScript
    {
        public const Int32 Id = 0098;

        private readonly BattleCalculator _v;

        public SimpleAttackGaiaScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if ((_v.Target.PlayerCategory & CharacterCategory.Terra) == 0)
            {
                _v.NormalPhysicalParams();
                _v.Caster.EnemyTranceBonusAttack();
                _v.CalcHpDamage();
            }
            else
            {
                _v.Context.Flags |= BattleCalcFlags.Miss;
            }
        }
    }
}
