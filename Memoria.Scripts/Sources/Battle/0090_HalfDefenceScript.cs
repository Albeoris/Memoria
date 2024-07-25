using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// ???
    /// </summary>
    [BattleScript(Id)]
    public sealed class HalfDefenceScript : IBattleScript
    {
        public const Int32 Id = 0090;

        private readonly BattleCalculator _v;

        public HalfDefenceScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, "PhysicalDefence", _v.Target.PhysicalDefence / 2, "MagicDefence", _v.Target.MagicDefence / 2);
        }
    }
}
