using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Swallow, Snort
    /// </summary>
    [BattleScript(Id)]
    public sealed class SwallowScript : IBattleScript
    {
        public const Int32 Id = 0106;

        private readonly BattleCalculator _v;

        public SwallowScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Remove();
        }
    }
}