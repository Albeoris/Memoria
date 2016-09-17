using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// HP Switching
    /// </summary>
    [BattleScript(Id)]
    public sealed class HPSwitchingScript : IBattleScript
    {
        public const Int32 Id = 0089;

        private readonly BattleCalculator _v;

        public HPSwitchingScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CheckUnsafetyOrMiss() || !_v.Target.CanBeAttacked())
                return;

            Int32 hp = _v.Caster.CurrentHp;
            _v.Caster.CurrentHp = _v.Target.CurrentHp;
            if (_v.Caster.CurrentHp > _v.Caster.MaximumHp)
                _v.Caster.CurrentHp = _v.Caster.MaximumHp;

            _v.Target.CurrentHp = (UInt16)hp;
            if (_v.Target.CurrentHp > _v.Target.MaximumHp)
                _v.Target.CurrentHp = _v.Target.MaximumHp;
        }
    }
}