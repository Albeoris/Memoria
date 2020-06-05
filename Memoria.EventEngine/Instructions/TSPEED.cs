using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTurnSpeed
    /// Change the entry's object turn speed.
    /// 
    /// 1st argument: turn speed (1 is slowest).
    /// AT_USPIN Speed (1 bytes)
    /// TSPEED = 0x099,
    /// </summary>
    internal sealed class TSPEED : JsmInstruction
    {
        private readonly IJsmExpression _speed;

        private TSPEED(IJsmExpression speed)
        {
            _speed = speed;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression speed = reader.ArgumentByte();
            return new TSPEED(speed);
        }
        public override String ToString()
        {
            return $"{nameof(TSPEED)}({nameof(_speed)}: {_speed})";
        }
    }
}