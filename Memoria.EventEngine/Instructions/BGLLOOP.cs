using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// MoveTileLoop
    /// Make the image of a field tile loop over space.
    /// 
    /// 1st argument: background tile block.
    /// 2nd argument: boolean on/off.
    /// 3rd and 4th arguments: speed in the X and Y directions.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_BOOL Activate (1 bytes)
    /// AT_SPIN X Loop (2 bytes)
    /// AT_SPIN Y Loop (2 bytes)
    /// BGLLOOP = 0x05C,
    /// </summary>
    internal sealed class BGLLOOP : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _activate;

        private readonly IJsmExpression _xLoop;

        private readonly IJsmExpression _yLoop;

        private BGLLOOP(IJsmExpression tileBlock, IJsmExpression activate, IJsmExpression xLoop, IJsmExpression yLoop)
        {
            _tileBlock = tileBlock;
            _activate = activate;
            _xLoop = xLoop;
            _yLoop = yLoop;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression activate = reader.ArgumentByte();
            IJsmExpression xLoop = reader.ArgumentInt16();
            IJsmExpression yLoop = reader.ArgumentInt16();
            return new BGLLOOP(tileBlock, activate, xLoop, yLoop);
        }
        public override String ToString()
        {
            return $"{nameof(BGLLOOP)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_activate)}: {_activate}, {nameof(_xLoop)}: {_xLoop}, {nameof(_yLoop)}: {_yLoop})";
        }
    }
}