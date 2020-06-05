using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xB5
    /// Seem to somehow make the object appropriate itself another entry's function list.
    /// 
    /// 1st argument: entry to get functions from.
    /// AT_ENTRY Entry (1 bytes)
    /// PRETEND = 0x0B5,
    /// </summary>
    internal sealed class PRETEND : JsmInstruction
    {
        private readonly IJsmExpression _entry;

        private PRETEND(IJsmExpression entry)
        {
            _entry = entry;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression entry = reader.ArgumentByte();
            return new PRETEND(entry);
        }
        public override String ToString()
        {
            return $"{nameof(PRETEND)}({nameof(_entry)}: {_entry})";
        }
    }
}