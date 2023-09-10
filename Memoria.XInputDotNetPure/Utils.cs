using System;

namespace XInputDotNetPure
{
    static class Utils
    {
        public const uint Success = 0x000;
        public const uint NotConnected = 0x000;

        private const int LeftStickDeadZone = 7849;
        private const int RightStickDeadZone = 8689;
        private const int TriggerDeadZone = 30;

        public static float ApplyTriggerDeadZone(byte value, GamePadDeadZone deadZoneMode)
        {
            if (deadZoneMode == GamePadDeadZone.None)
            {
                return ApplyDeadZone(value, byte.MaxValue, 0.0f);
            }
            else
            {
                return ApplyDeadZone(value, byte.MaxValue, TriggerDeadZone);
            }
        }

        public static GamePadThumbSticks.StickValue ApplyLeftStickDeadZone(short valueX, short valueY, GamePadDeadZone deadZoneMode)
        {
            return ApplyStickDeadZone(valueX, valueY, deadZoneMode, LeftStickDeadZone);
        }

        public static GamePadThumbSticks.StickValue ApplyRightStickDeadZone(short valueX, short valueY, GamePadDeadZone deadZoneMode)
        {
            return ApplyStickDeadZone(valueX, valueY, deadZoneMode, RightStickDeadZone);
        }

        private static GamePadThumbSticks.StickValue ApplyStickDeadZone(short valueX, short valueY, GamePadDeadZone deadZoneMode, int deadZoneSize)
        {
            if (deadZoneMode == GamePadDeadZone.Circular)
            {
                // Cast to long to avoid int overflow if valueX and valueY are both 32768, which would result in a negative number and Sqrt returns NaN
                float distanceFromCenter = (float)Math.Sqrt((long)valueX * (long)valueX + (long)valueY * (long)valueY);
                float coefficient = ApplyDeadZone(distanceFromCenter, short.MaxValue, deadZoneSize);
                coefficient = coefficient > 0.0f ? coefficient / distanceFromCenter : 0.0f;
                return new GamePadThumbSticks.StickValue(
                    Clamp(valueX * coefficient),
                    Clamp(valueY * coefficient)
                );
            }
            else if (deadZoneMode == GamePadDeadZone.IndependentAxes)
            {
                return new GamePadThumbSticks.StickValue(
                    ApplyDeadZone(valueX, short.MaxValue, deadZoneSize),
                    ApplyDeadZone(valueY, short.MaxValue, deadZoneSize)
                );
            }
            else
            {
                return new GamePadThumbSticks.StickValue(
                    ApplyDeadZone(valueX, short.MaxValue, 0.0f),
                    ApplyDeadZone(valueY, short.MaxValue, 0.0f)
                );
            }
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        private static float Clamp(float value)
        {
            return value < -1.0f ? -1.0f : (value > 1.0f ? 1.0f : value);
        }

        private static float ApplyDeadZone(float value, float maxValue, float deadZoneSize)
        {
            if (value < -deadZoneSize)
            {
                value += deadZoneSize;
            }
            else if (value > deadZoneSize)
            {
                value -= deadZoneSize;
            }
            else
            {
                return 0.0f;
            }

            value /= maxValue - deadZoneSize;

            return Clamp(value);
        }
    }
}
