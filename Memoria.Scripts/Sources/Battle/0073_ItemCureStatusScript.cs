using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Echo Screen, Antidote, Eye Drops, Magic Tag, Vaccine, Remedy, Annoyntment, Gysahl Greens
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemCureStatusScript : IBattleScript
    {
        public const Int32 Id = 0073;

        private readonly BattleCalculator _v;

        public ItemCureStatusScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.TargetCommand.TryRemoveItemStatuses();
        }
    }
}