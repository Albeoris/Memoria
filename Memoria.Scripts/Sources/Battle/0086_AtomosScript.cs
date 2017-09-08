using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Atomos
    /// </summary>
    [BattleScript(Id)]
    public sealed class AtomosScript : IBattleScript
    {
        public const Int32 Id = 0086;

        private readonly BattleCalculator _v;

        public AtomosScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CheckUnsafetyOrMiss())
                return;

            _v.SetCommandAttack();
            _v.CasterCommand.BonusElement();
            if (!_v.CanAttackMagic())
                return;

            _v.Context.Attack += (Int16)ff9item.FF9Item_GetCount(225);
            _v.TargetCommand.CalcProportionDamage();
        }
    }
}