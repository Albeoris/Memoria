using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// AttachObject
    /// Attach an object to another one.
    /// 
    /// 1st argument: carried object.
    /// 2nd argument: carrying object.
    /// 3rd argument: attachment point (unknown format).
    /// AT_ENTRY Object (1 bytes)
    /// AT_ENTRY Carrier (1 bytes)
    /// AT_USPIN Attachement Point (1 bytes)
    /// ATTACH = 0x04C,
    /// </summary>
    internal sealed class ATTACH : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _carrier;

        private readonly IJsmExpression _attachementPoint;

        private ATTACH(IJsmExpression @object, IJsmExpression carrier, IJsmExpression attachementPoint)
        {
            _object = @object;
            _carrier = carrier;
            _attachementPoint = attachementPoint;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression carrier = reader.ArgumentByte();
            IJsmExpression attachementPoint = reader.ArgumentByte();
            return new ATTACH(@object, carrier, attachementPoint);
        }
        public override String ToString()
        {
            return $"{nameof(ATTACH)}({nameof(_object)}: {_object}, {nameof(_carrier)}: {_carrier}, {nameof(_attachementPoint)}: {_attachementPoint})";
        }
    }
}