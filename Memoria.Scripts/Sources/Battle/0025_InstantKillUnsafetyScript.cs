using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Roulette, Photon
    /// </summary>
    [BattleScript(Id)]
    public sealed class InstantKillUnsafetyScript : IBattleScript
    {
        public const Int32 Id = 0025;

        private readonly BattleCalculator _v;

        public InstantKillUnsafetyScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.CheckUnsafetyOrMiss() && _v.Target.CanBeAttacked())
                _v.TargetCommand.InstantKill();
        }
    }
}