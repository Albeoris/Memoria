using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetWalkSpeed
    /// Change the walk speed.
    /// 
    /// 1st argument: speed (surely in unit/frame).
    /// AT_USPIN Speed (1 bytes)
    /// MSPEED = 0x026,
    /// </summary>
    internal sealed class MSPEED : JsmInstruction
    {
        private readonly IJsmExpression _speed;

        private MSPEED(IJsmExpression speed)
        {
            _speed = speed;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression speed = reader.ArgumentByte();
            return new MSPEED(speed);
        }
        public override String ToString()
        {
            return $"{nameof(MSPEED)}({nameof(_speed)}: {_speed})";
        }
    }
}