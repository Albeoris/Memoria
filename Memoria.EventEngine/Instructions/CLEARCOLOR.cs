using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetBackgroundColor
    /// Change the default color, seen behind the field's tiles.
    /// 
    /// 1st to 3rd arguments: color in (Red, Green, Blue) format.
    /// AT_COLOR_RED ColorR (1 bytes)
    /// AT_COLOR_GREEN ColorG (1 bytes)
    /// AT_COLOR_BLUE ColorB (1 bytes)
    /// CLEARCOLOR = 0x06B,
    /// </summary>
    internal sealed class CLEARCOLOR : JsmInstruction
    {
        private readonly IJsmExpression _colorR;

        private readonly IJsmExpression _colorG;

        private readonly IJsmExpression _colorB;

        private CLEARCOLOR(IJsmExpression colorR, IJsmExpression colorG, IJsmExpression colorB)
        {
            _colorR = colorR;
            _colorG = colorG;
            _colorB = colorB;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression colorR = reader.ArgumentByte();
            IJsmExpression colorG = reader.ArgumentByte();
            IJsmExpression colorB = reader.ArgumentByte();
            return new CLEARCOLOR(colorR, colorG, colorB);
        }
        public override String ToString()
        {
            return $"{nameof(CLEARCOLOR)}({nameof(_colorR)}: {_colorR}, {nameof(_colorG)}: {_colorG}, {nameof(_colorB)}: {_colorB})";
        }
    }
}