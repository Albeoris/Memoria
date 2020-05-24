using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetVibrationFlags
    /// Change the vibration flags.
    /// 
    /// 1st argument: flags.
    ///  8: Loop
    ///  16: Wrap
    /// AT_BOOLLIST Flags (1 bytes)
    /// VIBFLAG = 0x0FB,
    /// </summary>
    internal sealed class VIBFLAG : JsmInstruction
    {
        private readonly IJsmExpression _flags;

        private VIBFLAG(IJsmExpression flags)
        {
            _flags = flags;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression flags = reader.ArgumentByte();
            return new VIBFLAG(flags);
        }
        public override String ToString()
        {
            return $"{nameof(VIBFLAG)}({nameof(_flags)}: {_flags})";
        }
    }
}