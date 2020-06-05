using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTriangleFlagMask
    /// Set a bitmask for some of the walkmesh triangle flags.
    /// 
    /// 1st argument: flag mask.
    ///  7: disable restricted triangles
    ///  8: disable player-restricted triangles
    /// AT_BOOLLIST Flags (1 bytes)
    /// BGIMASK = 0x027,
    /// </summary>
    internal sealed class BGIMASK : JsmInstruction
    {
        private readonly IJsmExpression _flags;

        private BGIMASK(IJsmExpression flags)
        {
            _flags = flags;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression flags = reader.ArgumentByte();
            return new BGIMASK(flags);
        }
        public override String ToString()
        {
            return $"{nameof(BGIMASK)}({nameof(_flags)}: {_flags})";
        }
    }
}