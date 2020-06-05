using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetShadowOffset
    /// Change the offset between the entry's object and its shadow.
    /// 
    /// 1st argument: offset X.
    /// 2nd argument: offset Y.
    /// AT_SPIN Offset X (2 bytes)
    /// AT_SPIN Offset Y (2 bytes)
    /// SHADOWOFFSET = 0x082,
    /// </summary>
    internal sealed class SHADOWOFFSET : JsmInstruction
    {
        private readonly IJsmExpression _offsetX;

        private readonly IJsmExpression _offsetY;

        private SHADOWOFFSET(IJsmExpression offsetX, IJsmExpression offsetY)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression offsetX = reader.ArgumentInt16();
            IJsmExpression offsetY = reader.ArgumentInt16();
            return new SHADOWOFFSET(offsetX, offsetY);
        }
        public override String ToString()
        {
            return $"{nameof(SHADOWOFFSET)}({nameof(_offsetX)}: {_offsetX}, {nameof(_offsetY)}: {_offsetY})";
        }
    }
}