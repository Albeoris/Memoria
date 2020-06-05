using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x32
    /// Unused Opcode; bugs if used in the non-PSX versions.
    /// AT_SPIN Unknown (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// LOCATE = 0x032,
    /// </summary>
    internal sealed class LOCATE : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private LOCATE(IJsmExpression unknown1, IJsmExpression unknown2)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentByte();
            IJsmExpression unknown2 = reader.ArgumentByte();
            return new LOCATE(unknown1, unknown2);
        }
        public override String ToString()
        {
            return $"{nameof(LOCATE)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2})";
        }
    }
}