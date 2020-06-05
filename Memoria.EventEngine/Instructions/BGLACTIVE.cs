using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ShowTile
    /// Show or hide a field tile block.
    /// 
    /// 1st argument: background tile block.
    /// 2nd argument: boolean show/hide.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_BOOL Show (1 bytes)
    /// BGLACTIVE = 0x05B,
    /// </summary>
    internal sealed class BGLACTIVE : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _show;

        private BGLACTIVE(IJsmExpression tileBlock, IJsmExpression show)
        {
            _tileBlock = tileBlock;
            _show = show;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression show = reader.ArgumentByte();
            return new BGLACTIVE(tileBlock, show);
        }
        public override String ToString()
        {
            return $"{nameof(BGLACTIVE)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_show)}: {_show})";
        }
    }
}