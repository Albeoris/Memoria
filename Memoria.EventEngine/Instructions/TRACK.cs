using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x77
    /// Unknown Opcode.
    /// AT_SPIN Unknown (2 bytes)
    /// AT_SPIN Unknown (2 bytes)
    /// TRACK = 0x077,
    /// </summary>
    internal sealed class TRACK : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private TRACK(IJsmExpression unknown1, IJsmExpression unknown2)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentInt16();
            IJsmExpression unknown2 = reader.ArgumentInt16();
            return new TRACK(unknown1, unknown2);
        }
        public override String ToString()
        {
            return $"{nameof(TRACK)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2})";
        }
    }
}