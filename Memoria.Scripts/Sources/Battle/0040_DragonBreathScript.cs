using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Dragon Breath
    /// </summary>
    [BattleScript(Id)]
    public sealed class DragonBreathScript : IBattleScript
    {
        public const Int32 Id = 0040;

        private readonly BattleCalculator _v;

        public DragonBreathScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = (Int32)_v.Target.MaximumHp - (Int32)_v.Target.CurrentHp;

            foreach (BattleUnit unit in BattleState.EnumerateUnits())
            {
                _v.Target.Change(unit);

                if (!_v.Target.IsPlayer && _v.Target.IsSelected)
                {
                    SBattleCalculator.CalcResult(_v);
                    BattleState.Unit2DReq(unit);
                }
            }
            _v.Target.Flags = 0;
            _v.Target.MpDamage = 0;
        }
    }
}
