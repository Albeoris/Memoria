using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WaitAnimationEx
    /// Wait until the object's animation has ended.
    /// 
    /// 1st argument: object's entry.
    /// AT_ENTRY Object (1 bytes)
    /// DWAITANIM = 0x0BE,
    /// </summary>
    internal sealed class DWAITANIM : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private DWAITANIM(IJsmExpression @object)
        {
            _object = @object;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            return new DWAITANIM(@object);
        }
        public override String ToString()
        {
            return $"{nameof(DWAITANIM)}({nameof(_object)}: {_object})";
        }
    }
}