using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
    public class BehaviourMessage : ComponentMessage
    {
        public Boolean IsEnabled;

        public BehaviourMessage()
        {
        }

        public BehaviourMessage(Behaviour monoBehaviour)
            : base(monoBehaviour)
        {
            IsEnabled = monoBehaviour.enabled;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(IsEnabled);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            IsEnabled = br.ReadBoolean();
        }
    }
}