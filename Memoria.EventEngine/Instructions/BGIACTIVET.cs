using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnablePathTriangle
    /// Enable or disable a triangle of field pathing.
    /// 
    /// 1st argument: triangle ID.
    /// 2nd argument: boolean enable/disable.
    /// AT_WALKTRIANGLE Triangle ID (2 bytes)
    /// AT_BOOL Enable (1 bytes)
    /// BGIACTIVET = 0x09A,
    /// </summary>
    internal sealed class BGIACTIVET : JsmInstruction
    {
        private readonly IJsmExpression _triangleId;

        private readonly IJsmExpression _enable;

        private BGIACTIVET(IJsmExpression triangleId, IJsmExpression enable)
        {
            _triangleId = triangleId;
            _enable = enable;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression triangleId = reader.ArgumentInt16();
            IJsmExpression enable = reader.ArgumentByte();
            return new BGIACTIVET(triangleId, enable);
        }
        public override String ToString()
        {
            return $"{nameof(BGIACTIVET)}({nameof(_triangleId)}: {_triangleId}, {nameof(_enable)}: {_enable})";
        }
    }
}