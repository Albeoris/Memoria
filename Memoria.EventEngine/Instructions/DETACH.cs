using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// DetachObject
    /// Stop attaching an object to another one.
    /// 
    /// 1st argument: carried object.
    /// AT_ENTRY Object (1 bytes)
    /// DETACH = 0x04D,
    /// </summary>
    internal sealed class DETACH : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private DETACH(IJsmExpression @object)
        {
            _object = @object;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            return new DETACH(@object);
        }
        public override String ToString()
        {
            return $"{nameof(DETACH)}({nameof(_object)}: {_object})";
        }
    }
}