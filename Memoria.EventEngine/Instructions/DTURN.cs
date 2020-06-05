using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// TimedTurnEx
    /// Make an object face an angle (animated).
    /// 
    /// 1st argument: object's entry.
    /// 2nd argument: angle.
    /// 0 faces south, 64 faces west, 128 faces north and 192 faces east.
    /// 3rd argument: turn speed (1 is slowest).
    /// AT_ENTRY Object (1 bytes)
    /// AT_USPIN Angle (1 bytes)
    /// AT_USPIN Speed (1 bytes)
    /// DTURN = 0x0BB,
    /// </summary>
    internal sealed class DTURN : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _angle;

        private readonly IJsmExpression _speed;

        private DTURN(IJsmExpression @object, IJsmExpression angle, IJsmExpression speed)
        {
            _object = @object;
            _angle = angle;
            _speed = speed;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression angle = reader.ArgumentByte();
            IJsmExpression speed = reader.ArgumentByte();
            return new DTURN(@object, angle, speed);
        }
        public override String ToString()
        {
            return $"{nameof(DTURN)}({nameof(_object)}: {_object}, {nameof(_angle)}: {_angle}, {nameof(_speed)}: {_speed})";
        }
    }
}