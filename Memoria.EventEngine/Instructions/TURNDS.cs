using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Turn
    /// Make the character face an angle (animated). Speed is defaulted to 16.
    /// 
    /// 1st argument: angle.
    /// 0 faces south, 64 faces west, 128 faces north and 192 faces east.
    /// AT_USPIN Angle (1 bytes)
    /// TURNDS = 0x0A7,
    /// </summary>
    internal sealed class TURNDS : JsmInstruction
    {
        private readonly IJsmExpression _angle;

        private TURNDS(IJsmExpression angle)
        {
            _angle = angle;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression angle = reader.ArgumentByte();
            return new TURNDS(angle);
        }
        public override String ToString()
        {
            return $"{nameof(TURNDS)}({nameof(_angle)}: {_angle})";
        }
    }
}