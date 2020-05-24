using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xD4
    /// Unknown Opcode.
    /// AT_SPIN Unknown (2 bytes)
    /// AT_SPIN Unknown (2 bytes)
    /// AT_SPIN Unknown (2 bytes)
    /// ATTACHOFFSET = 0x0D4,
    /// </summary>
    internal sealed class ATTACHOFFSET : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private readonly IJsmExpression _unknown3;

        private ATTACHOFFSET(IJsmExpression unknown1, IJsmExpression unknown2, IJsmExpression unknown3)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
            _unknown3 = unknown3;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentInt16();
            IJsmExpression unknown2 = reader.ArgumentInt16();
            IJsmExpression unknown3 = reader.ArgumentInt16();
            return new ATTACHOFFSET(unknown1, unknown2, unknown3);
        }
        public override String ToString()
        {
            return $"{nameof(ATTACHOFFSET)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2}, {nameof(_unknown3)}: {_unknown3})";
        }
    }
}