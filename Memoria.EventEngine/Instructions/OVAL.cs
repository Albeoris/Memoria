using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xDF
    /// Unknown Opcode.
    /// AT_USPIN Unknown (1 bytes)
    /// OVAL = 0x0DF,
    /// </summary>
    internal sealed class OVAL : JsmInstruction
    {
        private readonly IJsmExpression _unknown;

        private OVAL(IJsmExpression unknown)
        {
            _unknown = unknown;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown = reader.ArgumentByte();
            return new OVAL(unknown);
        }
        public override String ToString()
        {
            return $"{nameof(OVAL)}({nameof(_unknown)}: {_unknown})";
        }
    }
}