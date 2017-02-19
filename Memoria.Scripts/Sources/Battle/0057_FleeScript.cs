using System;
using FF9;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Flee
    /// </summary>
    [BattleScript(Id)]
    public sealed class FleeScript : IBattleScript
    {
        public const Int32 Id = 0057;

        private readonly BattleCalculator _v;

        public FleeScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            btl_sys.CheckEscape(false);
            if (_v.CanEscape())
                BattleState.EnqueueCommand(BattleState.EscapeCommand, (BattleCommandId)FleeIterationScript.Id, 0U, 15, true);
        }
    }
}