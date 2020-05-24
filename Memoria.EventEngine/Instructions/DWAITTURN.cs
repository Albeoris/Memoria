using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WaitTurnEx
    /// Wait until an object facing movement has ended.
    /// 
    /// 1st argument: object's entry.
    /// AT_ENTRY Object (1 bytes)
    /// DWAITTURN = 0x0BC,
    /// </summary>
    internal sealed class DWAITTURN : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private DWAITTURN(IJsmExpression @object)
        {
            _object = @object;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            return new DWAITTURN(@object);
        }
        public override String ToString()
        {
            return $"{nameof(DWAITTURN)}({nameof(_object)}: {_object})";
        }
    }
}