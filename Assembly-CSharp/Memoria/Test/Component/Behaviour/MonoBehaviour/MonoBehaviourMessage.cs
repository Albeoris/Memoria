using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
    public class MonoBehaviourMessage : BehaviourMessage
    {
        public Boolean UseGUILayout;

        public MonoBehaviourMessage()
        {
        }

        public MonoBehaviourMessage(MonoBehaviour monoBehaviour)
            : base(monoBehaviour)
        {
            UseGUILayout = monoBehaviour.useGUILayout;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(UseGUILayout);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            UseGUILayout = br.ReadBoolean();
        }
    }
}
