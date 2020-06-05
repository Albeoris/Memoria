using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xA3
    /// Unknown Opcode.
    /// AT_SPIN Unknown (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// DRADIUS = 0x0A3,
    /// </summary>
    internal sealed class DRADIUS : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private readonly IJsmExpression _unknown3;

        private readonly IJsmExpression _unknown4;

        private DRADIUS(IJsmExpression unknown1, IJsmExpression unknown2, IJsmExpression unknown3, IJsmExpression unknown4)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
            _unknown3 = unknown3;
            _unknown4 = unknown4;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentByte();
            IJsmExpression unknown2 = reader.ArgumentByte();
            IJsmExpression unknown3 = reader.ArgumentByte();
            IJsmExpression unknown4 = reader.ArgumentByte();
            return new DRADIUS(unknown1, unknown2, unknown3, unknown4);
        }
        public override String ToString()
        {
            return $"{nameof(DRADIUS)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2}, {nameof(_unknown3)}: {_unknown3}, {nameof(_unknown4)}: {_unknown4})";
        }
    }
}