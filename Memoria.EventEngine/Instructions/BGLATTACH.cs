using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// AttachTile
    /// Make a part of the field background follow the player's movements. Also apply a color filter out of that tile block's range.
    /// 
    /// 1st argument: tile block.
    /// 2nd and 3rd arguments: offset position in (X, Y) format.
    /// 4th argument: filter mode ; use -1 for no filter effect.
    /// 5th to 7th arguments: filter color in (Red, Green, Blue) format.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_SPIN Position X (2 bytes)
    /// AT_SPIN Position Y (1 bytes)
    /// AT_SPIN Filter Mode (1 bytes)
    /// AT_COLOR_RED Filter ColorR (1 bytes)
    /// AT_COLOR_GREEN Filter ColorG (1 bytes)
    /// AT_COLOR_BLUE Filter ColorB (1 bytes)
    /// BGLATTACH = 0x092,
    /// </summary>
    internal sealed class BGLATTACH : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _positionX;

        private readonly IJsmExpression _positionY;

        private readonly IJsmExpression _filterMode;

        private readonly IJsmExpression _filterColorR;

        private readonly IJsmExpression _filterColorG;

        private readonly IJsmExpression _filterColorB;

        private BGLATTACH(IJsmExpression tileBlock, IJsmExpression positionX, IJsmExpression positionY, IJsmExpression filterMode, IJsmExpression filterColorR, IJsmExpression filterColorG, IJsmExpression filterColorB)
        {
            _tileBlock = tileBlock;
            _positionX = positionX;
            _positionY = positionY;
            _filterMode = filterMode;
            _filterColorR = filterColorR;
            _filterColorG = filterColorG;
            _filterColorB = filterColorB;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression positionX = reader.ArgumentInt16();
            IJsmExpression positionY = reader.ArgumentByte();
            IJsmExpression filterMode = reader.ArgumentByte();
            IJsmExpression filterColorR = reader.ArgumentByte();
            IJsmExpression filterColorG = reader.ArgumentByte();
            IJsmExpression filterColorB = reader.ArgumentByte();
            return new BGLATTACH(tileBlock, positionX, positionY, filterMode, filterColorR, filterColorG, filterColorB);
        }
        public override String ToString()
        {
            return $"{nameof(BGLATTACH)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_positionX)}: {_positionX}, {nameof(_positionY)}: {_positionY}, {nameof(_filterMode)}: {_filterMode}, {nameof(_filterColorR)}: {_filterColorR}, {nameof(_filterColorG)}: {_filterColorG}, {nameof(_filterColorB)}: {_filterColorB})";
        }
    }
}