using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// TimedTurn
    /// Make the character face an angle (animated).
    /// 
    /// 1st argument: angle.
    /// 0 faces south, 64 faces west, 128 faces north and 192 faces east.
    /// 2nd argument: turn speed (1 is slowest).
    /// AT_USPIN Angle (1 bytes)
    /// AT_USPIN Speed (1 bytes)
    /// TURN = 0x056,
    /// </summary>
    internal sealed class TURN : JsmInstruction
    {
        private readonly IJsmExpression _angle;

        private readonly IJsmExpression _speed;

        private TURN(IJsmExpression angle, IJsmExpression speed)
        {
            _angle = angle;
            _speed = speed;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression angle = reader.ArgumentByte();
            IJsmExpression speed = reader.ArgumentByte();
            return new TURN(angle, speed);
        }
        public override String ToString()
        {
            return $"{nameof(TURN)}({nameof(_angle)}: {_angle}, {nameof(_speed)}: {_speed})";
        }
    }
}