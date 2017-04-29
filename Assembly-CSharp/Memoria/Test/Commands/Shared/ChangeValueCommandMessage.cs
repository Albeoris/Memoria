using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Memoria.Test
{
    internal sealed partial class ChangeValueCommandMessage : CommandMessage
    {
        public Int32 InstanceId;
        public String MemberName;
        public IValueMessage Value;

        public override void Serialize(BinaryWriter bw)
        {
            bw.Write(InstanceId);
            bw.Write(MemberName);
            ValueMessageFactory.Serialize(bw, Value);
        }

        public override void Deserialize(BinaryReader br)
        {
            InstanceId = br.ReadInt32();
            MemberName = br.ReadString();
            Value = ValueMessageFactory.Deserialize(br);
        }
    }
}
