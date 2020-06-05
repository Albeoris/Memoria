using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xC9
    /// Unknown Opcode.
    /// AT_USPIN Unknown (1 bytes)
    /// AT_USPIN Unknown (2 bytes)
    /// AT_USPIN Unknown (2 bytes)
    /// AT_USPIN Unknown (2 bytes)
    /// AT_USPIN Unknown (2 bytes)
    /// BGVDEFINE = 0x0C9,
    /// </summary>
    internal sealed class BGVDEFINE : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private readonly IJsmExpression _unknown3;

        private readonly IJsmExpression _unknown4;

        private readonly IJsmExpression _unknown5;

        private BGVDEFINE(IJsmExpression unknown1, IJsmExpression unknown2, IJsmExpression unknown3, IJsmExpression unknown4, IJsmExpression unknown5)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
            _unknown3 = unknown3;
            _unknown4 = unknown4;
            _unknown5 = unknown5;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentByte();
            IJsmExpression unknown2 = reader.ArgumentInt16();
            IJsmExpression unknown3 = reader.ArgumentInt16();
            IJsmExpression unknown4 = reader.ArgumentInt16();
            IJsmExpression unknown5 = reader.ArgumentInt16();
            return new BGVDEFINE(unknown1, unknown2, unknown3, unknown4, unknown5);
        }
        public override String ToString()
        {
            return $"{nameof(BGVDEFINE)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2}, {nameof(_unknown3)}: {_unknown3}, {nameof(_unknown4)}: {_unknown4}, {nameof(_unknown5)}: {_unknown5})";
        }
    }
}