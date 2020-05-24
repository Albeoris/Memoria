using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x87
    /// Unknown Opcode.
    /// AT_ENTRY Entry (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// DDIR = 0x087,
    /// </summary>
    internal sealed class DDIR : JsmInstruction
    {
        private readonly IJsmExpression _entry;

        private readonly IJsmExpression _unknown;

        private DDIR(IJsmExpression entry, IJsmExpression unknown)
        {
            _entry = entry;
            _unknown = unknown;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression entry = reader.ArgumentByte();
            IJsmExpression unknown = reader.ArgumentByte();
            return new DDIR(entry, unknown);
        }
        public override String ToString()
        {
            return $"{nameof(DDIR)}({nameof(_entry)}: {_entry}, {nameof(_unknown)}: {_unknown})";
        }
    }
}