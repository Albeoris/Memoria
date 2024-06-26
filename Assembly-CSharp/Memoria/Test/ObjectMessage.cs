using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
    public abstract class ObjectMessage : IRemotingMessage
    {
        //public Int32 CheckField1 = 0x77FFFF77;

        public Int32 InstanceId;
        public String TypeName;
        public String Name;
        public HideFlags HideFlags;

        public ObjectMessage()
        {
        }

        public ObjectMessage(UnityEngine.Object obj)
        {
            InstanceId = obj.GetInstanceID();
            TypeName = obj.GetType().Name;
            Name = obj.name;
            HideFlags = obj.hideFlags;
        }

        public virtual void Serialize(BinaryWriter bw)
        {
            //bw.Write(CheckField1);

            bw.Write(InstanceId);
            bw.Write(TypeName);
            bw.Write(Name);
            bw.Write((Int32)HideFlags);
        }

        public virtual void Deserialize(BinaryReader br)
        {
            //if (CheckField1 != br.ReadInt32())
            //    throw new InvalidOperationException();

            InstanceId = br.ReadInt32();
            TypeName = br.ReadString();
            Name = br.ReadString();
            HideFlags = (HideFlags)br.ReadInt32();
        }
    }
}
