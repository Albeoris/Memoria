using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetAnimationSpeed
    /// Set the current object's animation speed.
    /// 
    /// 1st argument: speed.
    /// AT_USPIN Speed (1 bytes)
    /// ASPEED = 0x03E,
    /// </summary>
    internal sealed class ASPEED : JsmInstruction
    {
        private readonly IJsmExpression _speed;

        private ASPEED(IJsmExpression speed)
        {
            _speed = speed;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression speed = reader.ArgumentByte();
            return new ASPEED(speed);
        }
        public override String ToString()
        {
            return $"{nameof(ASPEED)}({nameof(_speed)}: {_speed})";
        }
    }
}