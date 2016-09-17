using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Flee (iteration)
    /// </summary>
    [BattleScript(Id)]
    public sealed class FleeIterationScript : IBattleScript
    {
        public const Int32 Id = 0056;

        private readonly BattleCalculator _v;

        public FleeIterationScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.CanEscape())
            {
                _v.TryEscape();
            }
        }
    }
}