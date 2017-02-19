using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Phoenix
    /// </summary>
    [BattleScript(Id)]
    public sealed class PhoenixScript : IBattleScript
    {
        public const Int32 Id = 0049;

        private readonly BattleCalculator _v;

        public PhoenixScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            MutableBattleCommand command = CreateCommand();
            Byte scriptId = PrepareCommand(command);
            SBattleCalculator.Calc(_v.Caster, _v.Target, command, scriptId);
        }

        private MutableBattleCommand CreateCommand()
        {
            return new MutableBattleCommand
            {
                Id = _v.Command.Id,
                IsShortSummon = _v.Command.IsShortSummon
            };
        }

        private Byte PrepareCommand(MutableBattleCommand command)
        {
            Byte scriptId;

            if (_v.Target.IsPlayer)
            {
                command.AbilityId = BattleAbilityId.RebirthFlame;
                scriptId = ReviveScript.Id;
            }
            else
            {
                command.AbilityId = BattleAbilityId.Phoenix;
                scriptId = MagicAttackScript.Id;
            }

            command.LoadAbility();
            return scriptId;
        }
    }
}