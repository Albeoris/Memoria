using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Matra Magic, Blue Shockwave, Judgment Sword, Helm Divide
    /// </summary>
    [BattleScript(Id)]
    public sealed class DirectHPDamageScript : IBattleScript
    {
        public const Int32 Id = 0027;

        private readonly BattleCalculator _v;

        public DirectHPDamageScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.CheckUnsafetyOrMiss() && _v.Target.CanBeAttacked())
            {
                _v.MagicAccuracy();
                _v.Target.PenaltyShellHitRate();
                if (_v.TryMagicHit())
                    _v.TryDirectHPDamage();
            }
        }
    }
}