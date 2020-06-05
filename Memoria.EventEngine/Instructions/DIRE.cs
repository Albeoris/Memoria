using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// TurnInstant
    /// Make the character face an angle.
    /// 
    /// 1st argument: angle.
    /// 0 faces south, 64 faces west, 128 faces north and 192 faces east.
    /// AT_USPIN Angle (1 bytes)
    /// DIRE = 0x036,
    /// </summary>
    internal sealed class DIRE : JsmInstruction
    {
        private readonly IJsmExpression _angle;

        private DIRE(IJsmExpression angle)
        {
            _angle = angle;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression angle = reader.ArgumentByte();
            return new DIRE(angle);
        }
        public override String ToString()
        {
            return $"{nameof(DIRE)}({nameof(_angle)}: {_angle})";
        }
    }
}