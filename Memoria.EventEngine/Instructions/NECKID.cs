using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetHeadAngle
    /// Maybe define the maximum angle and distance for the character to turn his head toward an active object.
    /// 
    /// 1st argument: unknown.
    /// 2nd argument: unknown.
    /// AT_USPIN Unknown (1 bytes)
    /// AT_USPIN Unknown (1 bytes)
    /// NECKID = 0x08B,
    /// </summary>
    internal sealed class NECKID : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private NECKID(IJsmExpression unknown1, IJsmExpression unknown2)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentByte();
            IJsmExpression unknown2 = reader.ArgumentByte();
            return new NECKID(unknown1, unknown2);
        }
        public override String ToString()
        {
            return $"{nameof(NECKID)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2})";
        }
    }
}