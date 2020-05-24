using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xCA
    /// Unknown Opcode.
    /// AT_SPIN Unknown (1 bytes)
    /// AT_BOOL Visible (1 bytes)
    /// BGAVISIBLE = 0x0CA,
    /// </summary>
    internal sealed class BGAVISIBLE : JsmInstruction
    {
        private readonly IJsmExpression _unknown;

        private readonly IJsmExpression _visible;

        private BGAVISIBLE(IJsmExpression unknown, IJsmExpression visible)
        {
            _unknown = unknown;
            _visible = visible;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown = reader.ArgumentByte();
            IJsmExpression visible = reader.ArgumentByte();
            return new BGAVISIBLE(unknown, visible);
        }
        public override String ToString()
        {
            return $"{nameof(BGAVISIBLE)}({nameof(_unknown)}: {_unknown}, {nameof(_visible)}: {_visible})";
        }
    }
}