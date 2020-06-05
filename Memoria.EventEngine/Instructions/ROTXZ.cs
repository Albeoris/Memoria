using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetPitchAngle
    /// Turn the model in the up/down direction.
    /// 
    /// 1st argument: angle (pitch axis).
    /// 2nd argument: angle (XZ axis).
    /// AT_USPIN Pitch (1 bytes)
    /// AT_USPIN XZ Angle (1 bytes)
    /// ROTXZ = 0x037,
    /// </summary>
    internal sealed class ROTXZ : JsmInstruction
    {
        private readonly IJsmExpression _pitch;

        private readonly IJsmExpression _xzAngle;

        private ROTXZ(IJsmExpression pitch, IJsmExpression xzAngle)
        {
            _pitch = pitch;
            _xzAngle = xzAngle;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression pitch = reader.ArgumentByte();
            IJsmExpression xzAngle = reader.ArgumentByte();
            return new ROTXZ(pitch, xzAngle);
        }
        public override String ToString()
        {
            return $"{nameof(ROTXZ)}({nameof(_pitch)}: {_pitch}, {nameof(_xzAngle)}: {_xzAngle})";
        }
    }
}