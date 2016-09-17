using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Detect
    /// </summary>
    [BattleScript(Id)]
    public sealed class DetectScript : IBattleScript
    {
        public const Int32 Id = 0060;

        private readonly BattleCalculator _v;

        public DetectScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Detect();
        }
    }
}