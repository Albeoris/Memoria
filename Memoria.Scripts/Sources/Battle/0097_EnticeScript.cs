using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Entice
    /// </summary>
    [BattleScript(Id)]
    public sealed class EnticeScript : IBattleScript
    {
        public const Int32 Id = 0097;

        private readonly BattleCalculator _v;

        public EnticeScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if ((_v.Target.PlayerCategory & CharacterCategory.Female) == 0)
                _v.TargetCommand.TryAlterCommandStatuses();
            else
                _v.Context.Flags |= BattleCalcFlags.Miss;
        }
    }
}