using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetWeather
    /// Add a foreground effect.
    /// 
    /// 1st argument: unknown.
    /// 2nd argument: unknown.
    /// AT_SPIN Unknown (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// RAIN = 0x0D8,
    /// </summary>
    internal sealed class RAIN : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private RAIN(IJsmExpression unknown1, IJsmExpression unknown2)
        {
            _unknown1 = unknown1;
            _unknown2 = unknown2;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentByte();
            IJsmExpression unknown2 = reader.ArgumentByte();
            return new RAIN(unknown1, unknown2);
        }
        public override String ToString()
        {
            return $"{nameof(RAIN)}({nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2})";
        }
    }
}