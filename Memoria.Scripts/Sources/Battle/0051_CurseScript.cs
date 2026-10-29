using Memoria.Data;
using System;
using System.Linq;

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
            if (_v.Command.ElementEx.Count == 0)
                return;
            _v.Target.WeakElementEx.AddMultiple(_v.Command.ElementEx);
            _v.Target.GuardElementEx.ExceptWith(_v.Command.ElementEx);
            _v.Target.AbsorbElementEx.ExceptWith(_v.Command.ElementEx);
            _v.Target.HalfElementEx.ExceptWith(_v.Command.ElementEx);

            UiState.SetBattleFollowMessage(BattleMesages.BecameWeakAgainst, _v.Command.ElementEx.First());
        }
    }
}
