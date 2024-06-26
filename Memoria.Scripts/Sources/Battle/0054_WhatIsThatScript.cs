using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// What’s That!?
    /// </summary>
    [BattleScript(Id)]
    public sealed class WhatIsThatScript : IBattleScript
    {
        public const Int32 Id = 0054;

        private readonly BattleCalculator _v;

        public WhatIsThatScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.IsPlayer && BattleState.IsSpecialStart)
            {
                _v.Context.Flags |= BattleCalcFlags.Miss;
                return;
            }

            _v.Target.FaceAsUnit(_v.Caster);
            _v.Target.ChangeRowToDefault();
        }
    }
}