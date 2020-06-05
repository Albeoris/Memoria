using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetObjectFlags
    /// Change flags of the current entry's object.
    /// 
    /// 1st argument: object flags.
    ///  1: show model
    ///  2: unknown
    ///  4: unknown
    ///  8: unknown
    ///  16: unknown
    ///  32: unknown
    /// AT_BOOLLIST Flags (1 bytes)
    /// CFLAG = 0x093,
    /// </summary>
    internal sealed class CFLAG : JsmInstruction
    {
        private readonly IJsmExpression _flags;

        private CFLAG(IJsmExpression flags)
        {
            _flags = flags;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression flags = reader.ArgumentByte();
            return new CFLAG(flags);
        }
        public override String ToString()
        {
            return $"{nameof(CFLAG)}({nameof(_flags)}: {_flags})";
        }
    }
}