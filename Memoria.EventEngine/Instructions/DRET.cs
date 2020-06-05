using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ReturnEntryFunctions
    /// Make all the currently executed functions return for a given entry.
    /// 
    /// 1st argument: entry for which functions are returned.
    /// AT_ENTRY Entry (1 bytes)
    /// DRET = 0x097,
    /// </summary>
    internal sealed class DRET : JsmInstruction
    {
        private readonly IJsmExpression _entry;

        private DRET(IJsmExpression entry)
        {
            _entry = entry;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression entry = reader.ArgumentByte();
            return new DRET(entry);
        }
        public override String ToString()
        {
            return $"{nameof(DRET)}({nameof(_entry)}: {_entry})";
        }
    }
}