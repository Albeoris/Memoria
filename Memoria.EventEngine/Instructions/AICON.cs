using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ATE
    /// Enable or disable ATE.
    /// 
    /// 1st argument: maybe flags (unknown format).
    /// AT_SPIN Unknown (1 bytes)
    /// AICON = 0x0D7,
    /// </summary>
    internal sealed class AICON : JsmInstruction
    {
        private readonly IJsmExpression _unknown;

        private AICON(IJsmExpression unknown)
        {
            _unknown = unknown;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown = reader.ArgumentByte();
            return new AICON(unknown);
        }
        public override String ToString()
        {
            return $"{nameof(AICON)}({nameof(_unknown)}: {_unknown})";
        }
    }
}