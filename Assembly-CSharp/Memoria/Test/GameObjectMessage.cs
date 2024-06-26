using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
    public sealed class GameObjectMessage : ObjectMessage
    {
        public const RemotingMessageType Type = RemotingMessageType.GameObject;

        public Int32 OrderedNumber;

        public String Tag;
        public Boolean IsActive;
        public Boolean IsActiveInHierarchy;
        public Int32 ParentInstanceId;

        public GameObjectMessage()
        {
        }

        public GameObjectMessage(GameObject gameObject)
            : base(gameObject)
        {
            Tag = gameObject.tag;
            IsActive = gameObject.activeSelf;
            IsActiveInHierarchy = gameObject.activeInHierarchy;

            ParentInstanceId = gameObject.transform?.parent?.gameObject.GetInstanceID() ?? 0;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(OrderedNumber);

            bw.Write(Tag);
            bw.Write(IsActive);
            bw.Write(IsActiveInHierarchy);
            bw.Write(ParentInstanceId);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            OrderedNumber = br.ReadInt32();

            Tag = br.ReadString();
            IsActive = br.ReadBoolean();
            IsActiveInHierarchy = br.ReadBoolean();
            ParentInstanceId = br.ReadInt32();
        }
    }
}
