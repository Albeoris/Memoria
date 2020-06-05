using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetDialogProgression
    /// Change the dialog progression value.
    /// 
    /// 1st argument: new value.
    /// AT_SPIN Progression (1 bytes)
    /// SETSIGNAL = 0x0E3,
    /// </summary>
    internal sealed class SETSIGNAL : JsmInstruction
    {
        private readonly IJsmExpression _progression;

        private SETSIGNAL(IJsmExpression progression)
        {
            _progression = progression;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression progression = reader.ArgumentByte();
            return new SETSIGNAL(progression);
        }
        public override String ToString()
        {
            return $"{nameof(SETSIGNAL)}({nameof(_progression)}: {_progression})";
        }
    }
}