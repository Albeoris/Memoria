using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x4F
    /// Unknown Opcode.
    /// AT_SPIN Unknown (1 bytes)
    /// STOP = 0x04F,
    /// </summary>
    internal sealed class STOP : JsmInstruction
    {
        private readonly IJsmExpression _unknown;

        private STOP(IJsmExpression unknown)
        {
            _unknown = unknown;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown = reader.ArgumentByte();
            return new STOP(unknown);
        }
        public override String ToString()
        {
            return $"{nameof(STOP)}({nameof(_unknown)}: {_unknown})";
        }
    }
}