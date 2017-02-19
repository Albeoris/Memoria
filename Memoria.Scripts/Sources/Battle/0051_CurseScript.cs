using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Curse
    /// </summary>
    [BattleScript(Id)]
    public sealed class CurseScript : IBattleScript
    {
        public const Int32 Id = 0051;

        private readonly BattleCalculator _v;

        public CurseScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.WeakElement |= _v.Command.Element;
            _v.Target.GuardElement &= _v.Command.Element;
            _v.Target.AbsorbElement &= _v.Command.Element;
            _v.Target.HalfElement &= _v.Command.Element;

            UiState.SetBattleFollowMessage(BattleMesages.BecameWeakAgainst, _v.Command.Element);
        }
    }
}