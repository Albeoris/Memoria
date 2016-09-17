using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Dragon’s Crest
    /// </summary>
    [BattleScript(Id)]
    public sealed class DragonCrestScript : IBattleScript
    {
        public const Int32 Id = 0068;

        private readonly BattleCalculator _v;

        public DragonCrestScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = (Int16)Math.Min(9999, GameState.Dragons * GameState.Dragons);
        }
    }
}