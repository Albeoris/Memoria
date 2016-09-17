using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// LV5 Death, Smash, Climhazzard(Story), Stock Break
    /// </summary>
    [BattleScript(Id)]
    public sealed class InstantKillScript : IBattleScript
    {
        public const Int32 Id = 0022;

        private readonly BattleCalculator _v;

        public InstantKillScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.TargetCommand.IsTargetLevelMultipleOfCommandRate() && _v.Target.CanBeAttacked())
                _v.TargetCommand.InstantKill();
        }
    }
}