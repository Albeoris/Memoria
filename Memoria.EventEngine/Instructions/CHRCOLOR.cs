using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetModelColor
    /// Change a 3D model's color.
    /// 
    /// 1st argument: entry associated with the model.
    /// 2nd to 4th arguments: color in (Red, Green, Blue) format.
    /// AT_ENTRY Object (1 bytes)
    /// AT_COLOR_RED ColorR (1 bytes)
    /// AT_COLOR_GREEN ColorG (1 bytes)
    /// AT_COLOR_BLUE ColorB (1 bytes)
    /// CHRCOLOR = 0x08F,
    /// </summary>
    internal sealed class CHRCOLOR : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _colorR;

        private readonly IJsmExpression _colorG;

        private readonly IJsmExpression _colorB;

        private CHRCOLOR(IJsmExpression @object, IJsmExpression colorR, IJsmExpression colorG, IJsmExpression colorB)
        {
            _object = @object;
            _colorR = colorR;
            _colorG = colorG;
            _colorB = colorB;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression colorR = reader.ArgumentByte();
            IJsmExpression colorG = reader.ArgumentByte();
            IJsmExpression colorB = reader.ArgumentByte();
            return new CHRCOLOR(@object, colorR, colorG, colorB);
        }
        public override String ToString()
        {
            return $"{nameof(CHRCOLOR)}({nameof(_object)}: {_object}, {nameof(_colorR)}: {_colorR}, {nameof(_colorG)}: {_colorG}, {nameof(_colorB)}: {_colorB})";
        }
    }
}