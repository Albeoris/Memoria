using System;
using System.IO;

namespace Memoria.Test
{
    internal sealed partial class DuplicateCommandMessage : CommandMessage
    {
        public Int32 InstanceId;

        public override void Serialize(BinaryWriter bw)
        {
            bw.Write(InstanceId);
        }

        public override void Deserialize(BinaryReader br)
        {
            InstanceId = br.ReadInt32();
        }
    }
}
