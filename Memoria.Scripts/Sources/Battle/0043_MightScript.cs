using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Might
    /// </summary>
    [BattleScript(Id)]
    public sealed class MightScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0043;

        private readonly BattleCalculator _v;

        public MightScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Strength = (Byte)Math.Min(99, _v.Target.Strength + _v.Target.Strength / _v.Command.Power);
        }

        public Single RateTarget()
        {
            return (Byte)Math.Min(99, _v.Target.Strength + _v.Target.Strength / _v.Command.Power);
        }
    }
}