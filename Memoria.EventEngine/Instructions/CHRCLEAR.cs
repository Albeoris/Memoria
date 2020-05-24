using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xCD
    /// Unknown Opcode.
    /// AT_BOOLLIST Flag List (2 bytes)
    /// CHRCLEAR = 0x0CD,
    /// </summary>
    internal sealed class CHRCLEAR : JsmInstruction
    {
        private readonly IJsmExpression _flagList;

        private CHRCLEAR(IJsmExpression flagList)
        {
            _flagList = flagList;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression flagList = reader.ArgumentInt16();
            return new CHRCLEAR(flagList);
        }
        public override String ToString()
        {
            return $"{nameof(CHRCLEAR)}({nameof(_flagList)}: {_flagList})";
        }
    }
}