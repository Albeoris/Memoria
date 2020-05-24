using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunTimer
    /// Run or pause the timer window.
    /// 
    /// 1st argument: boolean run/pause.
    /// AT_BOOL Run (1 bytes)
    /// TIMERCONTROL = 0x07D,
    /// </summary>
    internal sealed class TIMERCONTROL : JsmInstruction
    {
        private readonly IJsmExpression _run;

        private TIMERCONTROL(IJsmExpression run)
        {
            _run = run;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression run = reader.ArgumentByte();
            return new TIMERCONTROL(run);
        }
        public override String ToString()
        {
            return $"{nameof(TIMERCONTROL)}({nameof(_run)}: {_run})";
        }
    }
}