using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTileColor
    /// Change the color of a field tile block.
    /// 
    /// 1st argument: background tile block.
    /// 2nd to 4th arguments: color in (Cyan, Magenta, Yellow) format.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_COLOR_CYAN ColorC (1 bytes)
    /// AT_COLOR_MAGENTA ColorM (1 bytes)
    /// AT_COLOR_YELLOW ColorY (1 bytes)
    /// BGLCOLOR = 0x059,
    /// </summary>
    internal sealed class BGLCOLOR : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _colorC;

        private readonly IJsmExpression _colorM;

        private readonly IJsmExpression _colorY;

        private BGLCOLOR(IJsmExpression tileBlock, IJsmExpression colorC, IJsmExpression colorM, IJsmExpression colorY)
        {
            _tileBlock = tileBlock;
            _colorC = colorC;
            _colorM = colorM;
            _colorY = colorY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression colorC = reader.ArgumentByte();
            IJsmExpression colorM = reader.ArgumentByte();
            IJsmExpression colorY = reader.ArgumentByte();
            return new BGLCOLOR(tileBlock, colorC, colorM, colorY);
        }
        public override String ToString()
        {
            return $"{nameof(BGLCOLOR)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_colorC)}: {_colorC}, {nameof(_colorM)}: {_colorM}, {nameof(_colorY)}: {_colorY})";
        }
    }
}