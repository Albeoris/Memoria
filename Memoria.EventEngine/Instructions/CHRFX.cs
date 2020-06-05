using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunModelCode
    /// Run a model code.
    /// 
    /// 1st argument: model code.
    /// 2nd to 4th arguments: depends on the model code.
    ///  Texture Blend (blend mode) with blend mode being 1 for screen, 2 for multiply, 4 for Soft and 255 for a solid texture
    ///  Slice (boolean slice/unslice, value)
    ///  Enable Mirror (boolean enable/disable)
    ///  Mirror Position (X, Z, Y)
    ///  Mirror Normal (X, Z, Y)
    ///  Mirror Color (Red, Green, Blue)
    ///  Sound codes (Animation, Frame, Value)
    ///   For Add (Secondary) Sound, the 3rd argument is the sound ID
    ///   For Sound Random Pitch, the 3rd argument is a boolean random/not random
    ///   For Remove Sound, the 3rd argument is unused
    /// AT_MODELCODE Model Code (1 bytes)
    /// AT_SPIN Argument 1 (2 bytes)
    /// AT_SPIN Argument 2 (2 bytes)
    /// AT_SPIN Argument 3 (2 bytes)
    /// CHRFX = 0x088,
    /// </summary>
    internal sealed class CHRFX : JsmInstruction
    {
        private readonly IJsmExpression _modelCode;

        private readonly IJsmExpression _argument1;

        private readonly IJsmExpression _argument2;

        private readonly IJsmExpression _argument3;

        private CHRFX(IJsmExpression modelCode, IJsmExpression argument1, IJsmExpression argument2, IJsmExpression argument3)
        {
            _modelCode = modelCode;
            _argument1 = argument1;
            _argument2 = argument2;
            _argument3 = argument3;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression modelCode = reader.ArgumentByte();
            IJsmExpression argument1 = reader.ArgumentInt16();
            IJsmExpression argument2 = reader.ArgumentInt16();
            IJsmExpression argument3 = reader.ArgumentInt16();
            return new CHRFX(modelCode, argument1, argument2, argument3);
        }
        public override String ToString()
        {
            return $"{nameof(CHRFX)}({nameof(_modelCode)}: {_modelCode}, {nameof(_argument1)}: {_argument1}, {nameof(_argument2)}: {_argument2}, {nameof(_argument3)}: {_argument3})";
        }
    }
}