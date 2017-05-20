using System;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Eat, Cook
    /// </summary>
    [BattleScript(Id)]
    public sealed class EatScript : IBattleScript
    {
        public const Int32 Id = 0065;

        private readonly BattleCalculator _v;

        public EatScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CheckUnsafetyOrMiss() || !_v.Target.CanBeAttacked() || _v.Target.HasCategory(EnemyCategory.Humanoid))
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.CannotEat);
                return;
            }

            if (_v.Target.CurrentHp > _v.Target.MaximumHp / _v.Command.Power)
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.CannotEatStrong);
                return;
            }

            BattleEnemyPrototype enemyPrototype = BattleEnemyPrototype.Find(_v.Target);
            Byte blueMagicId = enemyPrototype.BlueMagicId;
            _v.Target.Kill();

            if (blueMagicId == 0 || ff9abil.FF9Abil_IsMaster(_v.Caster.PlayerIndex, blueMagicId))
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.TasteBad);
                return;
            }

            ff9abil.FF9Abil_SetMaster(_v.Caster.PlayerIndex, blueMagicId);
            BattleState.RaiseAbilitiesAchievement(blueMagicId);
            UiState.SetBattleFollowFormatMessage(BattleMesages.Learned, FF9TextTool.ActionAbilityName(blueMagicId));
        }
    }
}