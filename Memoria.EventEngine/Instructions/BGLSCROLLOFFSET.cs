using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xE4
    /// Seem to move field tile while applying a loop effect to it.
    /// 
    /// 1st argument: background tile block.
    /// 2nd argument: boolean activate/deactivate.
    /// 3rd and 4th arguments: seems to be the movement in (X/256, Y/256) format.
    /// 5th argument: unknown boolean.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_BOOL Enable (1 bytes)
    /// AT_SPIN Delta (2 bytes)
    /// AT_SPIN Offset (2 bytes)
    /// AT_BOOL Is X Offset (1 bytes)
    /// BGLSCROLLOFFSET = 0x0E4,
    /// </summary>
    internal sealed class BGLSCROLLOFFSET : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _enable;

        private readonly IJsmExpression _delta;

        private readonly IJsmExpression _offset;

        private readonly IJsmExpression _isXOffset;

        private BGLSCROLLOFFSET(IJsmExpression tileBlock, IJsmExpression enable, IJsmExpression delta, IJsmExpression offset, IJsmExpression isXOffset)
        {
            _tileBlock = tileBlock;
            _enable = enable;
            _delta = delta;
            _offset = offset;
            _isXOffset = isXOffset;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression enable = reader.ArgumentByte();
            IJsmExpression delta = reader.ArgumentInt16();
            IJsmExpression offset = reader.ArgumentInt16();
            IJsmExpression isXOffset = reader.ArgumentByte();
            return new BGLSCROLLOFFSET(tileBlock, enable, delta, offset, isXOffset);
        }
        public override String ToString()
        {
            return $"{nameof(BGLSCROLLOFFSET)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_enable)}: {_enable}, {nameof(_delta)}: {_delta}, {nameof(_offset)}: {_offset}, {nameof(_isXOffset)}: {_isXOffset})";
        }
    }
}