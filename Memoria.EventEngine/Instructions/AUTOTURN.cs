using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x91
    /// Unknown Opcode.
    /// AT_BOOL Unknown (1 bytes)
    /// AUTOTURN = 0x091,
    /// </summary>
    internal sealed class AUTOTURN : JsmInstruction
    {
        private readonly IJsmExpression _unknown;

        private AUTOTURN(IJsmExpression unknown)
        {
            _unknown = unknown;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown = reader.ArgumentByte();
            return new AUTOTURN(unknown);
        }
        public override String ToString()
        {
            return $"{nameof(AUTOTURN)}({nameof(_unknown)}: {_unknown})";
        }
    }
}