using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Death
    /// </summary>
    [BattleScript(Id)]
    public sealed class DeathScript : IBattleScript
    {
        public const Int32 Id = 0014;

        private readonly BattleCalculator _v;

        public DeathScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CheckUnsafetyOrGuard())
                return;

            if (_v.Target.IsZombie)
            {
                if (_v.Target.CanBeAttacked())
                    _v.Target.CurrentHp = _v.Target.MaximumHp;
                return;
            }

            _v.MagicAccuracy();
            if (_v.TargetCommand.TryMagicHit())
                _v.TargetCommand.TryAlterCommandStatuses();
        }
    }
}