using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xCC
    /// Maybe make the character transparent.
    /// AT_BOOLLIST Flag List (2 bytes)
    /// CHRSET = 0x0CC,
    /// </summary>
    internal sealed class CHRSET : JsmInstruction
    {
        private readonly IJsmExpression _flagList;

        private CHRSET(IJsmExpression flagList)
        {
            _flagList = flagList;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression flagList = reader.ArgumentInt16();
            return new CHRSET(flagList);
        }
        public override String ToString()
        {
            return $"{nameof(CHRSET)}({nameof(_flagList)}: {_flagList})";
        }
    }
}