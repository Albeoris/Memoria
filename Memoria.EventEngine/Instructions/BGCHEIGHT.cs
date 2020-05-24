using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetCameraFollowHeight
    /// Define the standard height gap between the player's character position and the camera view.
    /// 
    /// 1st argument: height.
    /// AT_SPIN Height (2 bytes)
    /// BGCHEIGHT = 0x072,
    /// </summary>
    internal sealed class BGCHEIGHT : JsmInstruction
    {
        private readonly IJsmExpression _height;

        private BGCHEIGHT(IJsmExpression height)
        {
            _height = height;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression height = reader.ArgumentInt16();
            return new BGCHEIGHT(height);
        }
        public override String ToString()
        {
            return $"{nameof(BGCHEIGHT)}({nameof(_height)}: {_height})";
        }
    }
}