using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Elexir
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemElexirScript : IBattleScript
    {
        public const Int32 Id = 0071;

        private readonly BattleCalculator _v;

        public ItemElexirScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CanBeAttacked())
                return;

            if (_v.Target.IsZombie)
            {
                _v.Target.CurrentMp = 0;
                _v.Target.Kill();
            }
            else
            {
                _v.Target.CurrentHp = _v.Target.MaximumHp;
                _v.Target.CurrentMp = _v.Target.MaximumMp;
            }
        }
    }
}