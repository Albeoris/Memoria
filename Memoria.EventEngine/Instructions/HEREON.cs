using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ShowHereIcon
    /// Show the Here icon over player's chatacter.
    /// 
    /// 1st argument: display type (0 to hide, 3 to show unconditionally)
    /// AT_SPIN Show (1 bytes)
    /// HEREON = 0x0EF,
    /// </summary>
    internal sealed class HEREON : JsmInstruction
    {
        private readonly IJsmExpression _show;

        private HEREON(IJsmExpression show)
        {
            _show = show;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression show = reader.ArgumentByte();
            return new HEREON(show);
        }
        public override String ToString()
        {
            return $"{nameof(HEREON)}({nameof(_show)}: {_show})";
        }
    }
}