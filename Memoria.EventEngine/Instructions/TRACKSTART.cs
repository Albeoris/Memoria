using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x76
    /// Unknown Opcode.
    /// AT_SPIN Unknown (2 bytes)
    /// AT_SPIN Unknown (2 bytes)
    /// TRACKSTART = 0x076,
    /// </summary>
    internal sealed class TRACKSTART : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private TRACKSTART(IJsmExpression unknown1, IJsmExpression unknown2)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentInt16();
            IJsmExpression unknown2 = reader.ArgumentInt16();
            return new TRACKSTART(unknown1, unknown2);
        }
        public override String ToString()
        {
            return $"{nameof(TRACKSTART)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2})";
        }
    }
}