using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Wait
    /// Wait some time.
    /// 
    /// 1st argument: amount of frames to wait.
    ///  For PAL, 1 frame is 0.04 seconds.
    ///  For other versions, 1 frame is about 0.033 seconds.
    /// AT_USPIN Frame Amount (1 bytes)
    /// WAIT = 0x022,
    /// </summary>
    internal sealed class WAIT : JsmInstruction
    {
        private readonly IJsmExpression _frameAmount;

        private WAIT(IJsmExpression frameAmount)
        {
            _frameAmount = frameAmount;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression frameAmount = reader.ArgumentByte();
            return new WAIT(frameAmount);
        }
        public override String ToString()
        {
            return $"{nameof(WAIT)}({nameof(_frameAmount)}: {_frameAmount})";
        }
    }
}