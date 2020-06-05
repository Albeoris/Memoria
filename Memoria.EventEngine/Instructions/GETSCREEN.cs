using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xA9
    /// Unknown Opcode.
    /// AT_ENTRY Unknown (1 bytes)
    /// GETSCREEN = 0x0A9,
    /// </summary>
    internal sealed class GETSCREEN : JsmInstruction
    {
        private readonly IJsmExpression _unknown;

        private GETSCREEN(IJsmExpression unknown)
        {
            _unknown = unknown;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown = reader.ArgumentByte();
            return new GETSCREEN(unknown);
        }
        public override String ToString()
        {
            return $"{nameof(GETSCREEN)}({nameof(_unknown)}: {_unknown})";
        }
    }
}