using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableHeadFocus
    /// Enable or disable the character turning his head toward an active object.
    /// 
    /// 1st argument: flags.
    ///  1: unknown
    ///  2: unknown
    ///  3: turn toward talkers
    /// AT_BOOLLIST Flags (1 bytes)
    /// NECKFLAG = 0x047,
    /// </summary>
    internal sealed class NECKFLAG : JsmInstruction
    {
        private readonly IJsmExpression _flags;

        private NECKFLAG(IJsmExpression flags)
        {
            _flags = flags;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression flags = reader.ArgumentByte();
            return new NECKFLAG(flags);
        }
        public override String ToString()
        {
            return $"{nameof(NECKFLAG)}({nameof(_flags)}: {_flags})";
        }
    }
}