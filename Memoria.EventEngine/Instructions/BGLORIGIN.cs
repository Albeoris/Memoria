using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTilePosition
    /// Move a field tile block.
    /// 
    /// 1st argument: background tile block.
    /// 2nd and 3rd argument: position in (X, Y) format.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_SPIN Position X (2 bytes)
    /// AT_SPIN Position Y (2 bytes)
    /// BGLORIGIN = 0x05E,
    /// </summary>
    internal sealed class BGLORIGIN : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _positionX;

        private readonly IJsmExpression _positionY;

        private BGLORIGIN(IJsmExpression tileBlock, IJsmExpression positionX, IJsmExpression positionY)
        {
            _tileBlock = tileBlock;
            _positionX = positionX;
            _positionY = positionY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression positionX = reader.ArgumentInt16();
            IJsmExpression positionY = reader.ArgumentInt16();
            return new BGLORIGIN(tileBlock, positionX, positionY);
        }
        public override String ToString()
        {
            return $"{nameof(BGLORIGIN)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_positionX)}: {_positionX}, {nameof(_positionY)}: {_positionY})";
        }
    }
}