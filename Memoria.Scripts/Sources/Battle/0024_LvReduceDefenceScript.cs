using System;
using System.Collections.Generic;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// LV3 Def-less
    /// </summary>
    [BattleScript(Id)]
    public sealed class LvReduceDefenceScript : IBattleScript
    {
        public const Int32 Id = 0024;

        private readonly BattleCalculator _v;

        public LvReduceDefenceScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.IsTargetLevelMultipleOfCommandRate() && _v.Target.CanBeAttacked())
            {
                List<Object> parameters = new List<Object>();
                if (_v.Target.PhysicalDefence != 0)
                {
                    parameters.Add("PhysicalDefence");
                    parameters.Add(GameRandom.Next16() % _v.Target.PhysicalDefence);
                }
                if (_v.Target.PhysicalDefence != 0)
                {
                    parameters.Add("MagicDefence");
                    parameters.Add(GameRandom.Next16() % _v.Target.MagicDefence);
                }
                if (parameters.Count > 0)
                    _v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, parameters.ToArray());
            }
        }
    }
}
