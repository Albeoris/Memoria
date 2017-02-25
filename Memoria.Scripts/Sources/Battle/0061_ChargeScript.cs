using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Charge
    /// </summary>
    [BattleScript(Id)]
    public sealed class ChargeScript : IBattleScript
    {
        public const Int32 Id = 0061;

        private readonly BattleCalculator _v;

        public ChargeScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            const BattleStatus cannotAttack = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Death | BattleStatus.Confuse | BattleStatus.Berserk
                                              | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze | BattleStatus.Jump;

            _v.Context.Flags = (BattleCalcFlags)BattleState.GetUnitIdsUnderStatus(false, BattleStatus.LowHP);
            if (_v.Context.Flags == 0)
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.ChargeFailed);
                return;
            }

            Boolean canAttack = false;
            foreach (BattleUnit unit in BattleState.EnumerateUnits())
            {
                if (((BattleCalcFlags)unit.Id & _v.Context.Flags) == 0 || unit.IsUnderStatus(cannotAttack))
                    continue;

                canAttack = true;
                UInt16 randomEnemy = BattleState.GetRandomUnitId(isPlayer: false);
                if (randomEnemy == 0)
                    return;

                BattleState.EnqueueConter(unit, (BattleCommandId)52, BattleAbilityId.Attack, randomEnemy);
            }

            if (canAttack == false)
            {
                _v.Context.Flags = 0;
                UiState.SetBattleFollowFormatMessage(BattleMesages.ChargeFailed);
            }
        }
    }
}