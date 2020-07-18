using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// White Draw
    /// </summary>
    [BattleScript(Id)]
    public sealed class WhiteDrawScript : IBattleScript
    {
        public const Int32 Id = 0041;

        private readonly BattleCalculator _v;

        public WhiteDrawScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CanBeHealed())
                return;

            _v.Caster.Flags = CalcFlag.MpAlteration | CalcFlag.MpRecovery;
            _v.Caster.MpDamage = GameRandom.Next16() % (_v.Target.Level * 2);

            foreach (BattleUnit unit in BattleState.EnumerateUnits())
            {
                if (!unit.IsPlayer || !unit.IsSelected)
                    continue;

                _v.Caster.Change(unit);
                SBattleCalculator.CalcResult(_v);
                BattleState.Unit2DReq(unit);
            }
            _v.Caster.Flags = 0;
            _v.Caster.MpDamage = 0;
        }
    }
}
