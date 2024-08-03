using Memoria.Data;
using System;

namespace Memoria
{
    /// <summary>The attribute required for all the battle ability scripts</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class BattleScriptAttribute : Attribute
    {
        public Int32 Id { get; }
        public BattleScriptAttribute(Int32 id) { Id = id; }
    }

    /// <summary>All the battle ability scripts should defined a constructor taking a BattleCalculator as unique argument</summary>
    public delegate IBattleScript BattleScriptFactory(BattleCalculator calc);

    /// <summary>The interface required for all the battle ability scripts</summary>
    public interface IBattleScript
    {
        void Perform();
    }

    /// <summary>The interface for rating battle ability scripts and determine which target is the best according to these estimations</summary>
    public interface IEstimateBattleScript
    {
        Single RateTarget();
    }

    public static class BattleScriptDamageEstimate
    {
        public static Single RateHpMp(Int32 currentValue, Int32 maxValue)
        {
            Single hpDiff = maxValue - currentValue;
            return hpDiff / maxValue;
        }
    }

    public static class BattleScriptAccuracyEstimate
    {
        public static Single RatePlayerAttackHit(Int32 hit)
        {
            return hit * 0.01f;
        }

        public static Single RatePlayerAttackEvade(Int32 eva)
        {
            return (100 - eva) * 0.01f;
        }
    }

    public static class BattleScriptStatusEstimate
    {
        public static Int32 RateStatuses(BattleStatus statuses)
        {
            if (statuses == 0)
                return 0;

            Int32 raiting = 0;
            foreach (BattleStatusId statusId in statuses.ToStatusList())
                raiting += RateStatus(statusId);

            return raiting;
        }

        public static Int32 RateStatus(BattleStatusId statusId)
        {
            switch (statusId)
            {
                case BattleStatusId.Death:
                    return -20;
                case BattleStatusId.Freeze:
                    return -19;
                case BattleStatusId.Heat:
                    return -18;
                case BattleStatusId.Venom:
                    return -17;
                case BattleStatusId.Stop:
                    return -16;
                case BattleStatusId.Petrify:
                    return -15;
                case BattleStatusId.Confuse:
                    return -14;
                case BattleStatusId.Berserk:
                    return -13;
                case BattleStatusId.GradualPetrify:
                    return -12;
                case BattleStatusId.Doom:
                    return -11;
                case BattleStatusId.LowHP:
                    return -10;
                case BattleStatusId.Sleep:
                    return -9;
                case BattleStatusId.Trouble:
                    return -8;
                case BattleStatusId.Zombie:
                    return -7;
                case BattleStatusId.Poison:
                    return -6;
                case BattleStatusId.Mini:
                    return -5;
                case BattleStatusId.Silence:
                    return -4;
                case BattleStatusId.Blind:
                    return -3;
                case BattleStatusId.Slow:
                    return -2;
                case BattleStatusId.Virus:
                    return -1;

                case BattleStatusId.EasyKill:
                    return 20;
                case BattleStatusId.AutoLife:
                    return 10;
                case BattleStatusId.Trance:
                    return 8;
                case BattleStatusId.Haste:
                    return 6;
                case BattleStatusId.Reflect:
                case BattleStatusId.Vanish:
                    return 5;
                case BattleStatusId.Regen:
                    return 4;
                case BattleStatusId.Shell:
                case BattleStatusId.Protect:
                    return 3;
                case BattleStatusId.Float:
                    return 2;
                case BattleStatusId.Jump:
                case BattleStatusId.Defend:
                    return 1;

                default:
                    return 0;
            }
        }
    }
}
