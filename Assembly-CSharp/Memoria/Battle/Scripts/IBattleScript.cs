using Memoria.Data;
using System;

namespace Memoria
{
    public delegate IBattleScript BattleScriptFactory(BattleCalculator calc);

    public interface IBattleScript
    {
        void Perform();
    }

    public interface IEstimateBattleScript
    {
        Single RateTarget();
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class BattleScriptAttribute : Attribute
    {
        public Int32 Id { get; }

        public BattleScriptAttribute(Int32 id)
        {
            Id = id;
        }
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
            for (Int32 i = 0; i < 32; i++)
            {
                BattleStatus status = (BattleStatus)(1u << i);
                if ((statuses & status) == status)
                    raiting += RateStatus(status);
            }

            return raiting;
        }

        public static Int32 RateStatus(BattleStatus status)
        {
            switch (status)
            {
                case 0:
                    return 0;

                case BattleStatus.Death:
                    return -20;
                case BattleStatus.Freeze:
                    return -19;
                case BattleStatus.Heat:
                    return -18;
                case BattleStatus.Venom:
                    return -17;
                case BattleStatus.Stop:
                    return -16;
                case BattleStatus.Petrify:
                    return -15;
                case BattleStatus.Confuse:
                    return -14;
                case BattleStatus.Berserk:
                    return -13;
                case BattleStatus.GradualPetrify:
                    return -12;
                case BattleStatus.Doom:
                    return -11;
                case BattleStatus.LowHP:
                    return -10;
                case BattleStatus.Sleep:
                    return -9;
                case BattleStatus.Trouble:
                    return -8;
                case BattleStatus.Zombie:
                    return -7;
                case BattleStatus.Poison:
                    return -6;
                case BattleStatus.Mini:
                    return -5;
                case BattleStatus.Silence:
                    return -4;
                case BattleStatus.Blind:
                    return -3;
                case BattleStatus.Slow:
                    return -2;
                case BattleStatus.Virus:
                    return -1;

                case BattleStatus.EasyKill:
                    return 20;
                case BattleStatus.AutoLife:
                    return 10;
                case BattleStatus.Trance:
                    return 8;
                case BattleStatus.Haste:
                    return 6;
                case BattleStatus.Reflect:
                case BattleStatus.Vanish:
                    return 5;
                case BattleStatus.Regen:
                    return 4;
                case BattleStatus.Shell:
                case BattleStatus.Protect:
                    return 3;
                case BattleStatus.Float:
                    return 2;
                case BattleStatus.Jump:
                case BattleStatus.Defend:
                    return 1;

                default:
                    return 0;
            }
        }
    }
}
