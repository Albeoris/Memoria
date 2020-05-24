using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetAnimationStandSpeed
    /// Change the standing animation speed.
    /// 
    /// 1st argument: unknown.
    /// 2nd argument: unknown.
    /// 3rd argument: unknown.
    /// 4th argument: unknown.
    /// AT_USPIN Unknown (1 bytes)
    /// AT_USPIN Unknown (1 bytes)
    /// AT_USPIN Unknown (1 bytes)
    /// AT_USPIN Unknown (1 bytes)
    /// IDLESPEED = 0x086,
    /// </summary>
    internal sealed class IDLESPEED : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private readonly IJsmExpression _unknown3;

        private readonly IJsmExpression _unknown4;

        private IDLESPEED(IJsmExpression unknown1, IJsmExpression unknown2, IJsmExpression unknown3, IJsmExpression unknown4)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
            _unknown3 = unknown3;
            _unknown4 = unknown4;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentByte();
            IJsmExpression unknown2 = reader.ArgumentByte();
            IJsmExpression unknown3 = reader.ArgumentByte();
            IJsmExpression unknown4 = reader.ArgumentByte();
            return new IDLESPEED(unknown1, unknown2, unknown3, unknown4);
        }
        public override String ToString()
        {
            return $"{nameof(IDLESPEED)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2}, {nameof(_unknown3)}: {_unknown3}, {nameof(_unknown4)}: {_unknown4})";
        }
    }
}