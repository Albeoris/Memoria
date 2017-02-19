using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Focus
    /// </summary>
    [BattleScript(Id)]
    public sealed class FocusScript : IBattleScript
    {
        public const Int32 Id = 0044;

        private readonly BattleCalculator _v;

        public FocusScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Magic = (Byte)Math.Min(99, _v.Target.Magic + _v.Target.Magic / _v.Command.Power);
        }
    }
}