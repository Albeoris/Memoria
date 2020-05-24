using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WalkTowardObject
    /// Make the character walk and follow an object.
    /// 
    /// 1st argument: object to walk toward.
    /// AT_ENTRY Object (1 bytes)
    /// MOVA = 0x024,
    /// </summary>
    internal sealed class MOVA : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private MOVA(IJsmExpression @object)
        {
            _object = @object;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            return new MOVA(@object);
        }
        public override String ToString()
        {
            return $"{nameof(MOVA)}({nameof(_object)}: {_object})";
        }
    }
}