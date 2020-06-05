using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ChangeTimerTime
    /// Change the remaining time of the timer window.
    /// 
    /// 1st argument: time in seconds.
    /// AT_USPIN Time (2 bytes)
    /// TIMERSET = 0x069,
    /// </summary>
    internal sealed class TIMERSET : JsmInstruction
    {
        private readonly IJsmExpression _time;

        private TIMERSET(IJsmExpression time)
        {
            _time = time;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression time = reader.ArgumentInt16();
            return new TIMERSET(time);
        }
        public override String ToString()
        {
            return $"{nameof(TIMERSET)}({nameof(_time)}: {_time})";
        }
    }
}