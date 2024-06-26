using System;
using Memoria;

public partial class FF9StateGlobal
{
    public sealed class FrogHandler
    {
        private const Int16 MaxReward = 99;

        private static readonly Int16[] Rewards = {2, 5, 9, 15, 23, 33, 45, MaxReward};

        public Int16 Number { get; private set; }

        public void Initialize(Int16 frogNumber)
        {
            Number = frogNumber;
        }

        public void Increment()
        {
            Int32 oldValue = Number;
            Int32 newValue = oldValue + Configuration.Hacks.FrogCatchingIncrement;

            if (newValue > MaxReward && newValue > Int16.MaxValue)
            {
                newValue = Int16.MaxValue;
            }
            else
            {
                for (Int32 i = 0; i < Rewards.Length; i++)
                {
                    Int16 maxValue = Rewards[i];

                    if (oldValue >= maxValue)
                        continue;

                    if (newValue > maxValue)
                        newValue = maxValue;

                    break;
                }
            }

            Number = (Int16)newValue;
            EMinigame.Catching99FrogAchievement(newValue);
        }
    }
}