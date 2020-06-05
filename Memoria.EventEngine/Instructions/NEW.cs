using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// InitCode
    /// Init a normal code (independant functions).
    /// 
    /// 1st argument: code entry to init.
    /// 2nd argument: Unique ID (defaulted to entry's ID if 0).
    /// AT_ENTRY Entry (1 bytes)
    /// AT_USPIN UID (1 bytes)
    /// NEW = 0x007,
    /// </summary>
    internal sealed class NEW : JsmInstruction
    {
        private readonly Byte _entry;

        private readonly Byte _uid;

        private NEW(Byte entry, Byte uid)
        {
            _entry = entry;
            _uid = uid;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            var entry = reader.Arguments();
            var uid = reader.ReadByte();
            return new NEW(entry, uid);
        }
        
        public override String ToString()
        {
            return $"{nameof(NEW)}({nameof(_entry)}: {_entry}, {nameof(_uid)}: {_entry})";
        }
    }
}