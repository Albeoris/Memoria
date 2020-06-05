using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunSPSCode
    /// Run Sps code, which seems to be special model effects on the field.
    /// 
    /// 1st argument: sps ID.
    /// 2nd argument: sps code.
    /// 3rd to 5th arguments: depend on the sps code.
    ///  Load Sps (sps type)
    ///  Enable Attribute (attribute list, boolean enable/disable)
    ///  Set Position (X, -Z, Y)
    ///  Set Rotation (angle X, angle Z, angle Y)
    ///  Set Scale (scale factor)
    ///  Attach (object's entry to attach, bone number)
    ///  Set Fade (fade)
    ///  Set Animation Rate (rate)
    ///  Set Frame Rate (rate)
    ///  Set Frame (value) where the value is factored by 16 to get the frame
    ///  Set Position Offset (X, -Z, Y)
    ///  Set Depth Offset (depth)
    /// AT_USPIN Sps (1 bytes)
    /// AT_SPSCODE Code (1 bytes)
    /// AT_SPIN Parameter 1 (2 bytes)
    /// AT_SPIN Parameter 2 (2 bytes)
    /// AT_SPIN Parameter 3 (2 bytes)
    /// SPS = 0x0B3,
    /// </summary>
    internal sealed class SPS : JsmInstruction
    {
        private readonly IJsmExpression _sps;

        private readonly IJsmExpression _code;

        private readonly IJsmExpression _parameter1;

        private readonly IJsmExpression _parameter2;

        private readonly IJsmExpression _parameter3;

        private SPS(IJsmExpression sps, IJsmExpression code, IJsmExpression parameter1, IJsmExpression parameter2, IJsmExpression parameter3)
        {
            _sps = sps;
            _code = code;
            _parameter1 = parameter1;
            _parameter2 = parameter2;
            _parameter3 = parameter3;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression sps = reader.ArgumentByte();
            IJsmExpression code = reader.ArgumentByte();
            IJsmExpression parameter1 = reader.ArgumentInt16();
            IJsmExpression parameter2 = reader.ArgumentInt16();
            IJsmExpression parameter3 = reader.ArgumentInt16();
            return new SPS(sps, code, parameter1, parameter2, parameter3);
        }
        public override String ToString()
        {
            return $"{nameof(SPS)}({nameof(_sps)}: {_sps}, {nameof(_code)}: {_code}, {nameof(_parameter1)}: {_parameter1}, {nameof(_parameter2)}: {_parameter2}, {nameof(_parameter3)}: {_parameter3})";
        }
    }
}