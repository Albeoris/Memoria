using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// InitObject
    /// Init an object code (associated with a 3D model). Also load the related model into the RAM.
    /// 
    /// 1st argument: code entry to init.
    /// 2nd argument: Unique ID (defaulted to entry's ID if 0).
    /// AT_ENTRY Entry (1 bytes)
    /// AT_USPIN UID (1 bytes)
    /// NEW3 = 0x009,
    /// </summary>
    internal sealed class NEW3 : JsmInstruction
    {
        private readonly Byte _entry;

        private readonly Byte _uid;

        private NEW3(Byte entry, Byte uid)
        {
            _entry = entry;
            _uid = uid;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            var entry = reader.Arguments();
            var uid = reader.ReadByte();
            return new NEW3(entry, uid);
        }
        
        public override String ToString()
        {
            return $"{nameof(NEW3)}({nameof(_entry)}: {_entry}, {nameof(_uid)}: {_uid})";
        }
    }
}