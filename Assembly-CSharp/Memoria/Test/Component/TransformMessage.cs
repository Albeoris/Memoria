using System.IO;
using UnityEngine;

namespace Memoria.Test
{
    public class TransformMessage : ComponentMessage
    {
        public Vector3 LocalPosition;
        public Vector3 LocalScale;
        public Vector3 LocalEulerAngles;
        public Quaternion LocalRotation;

        public TransformMessage()
        {
        }

        public TransformMessage(Transform transform)
            : base(transform)
        {
            LocalPosition = transform.localPosition;
            LocalEulerAngles = transform.localEulerAngles;
            LocalRotation = transform.localRotation;
            LocalScale = transform.localScale;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(LocalPosition);
            bw.Write(LocalScale);
            bw.Write(LocalEulerAngles);
            bw.Write(LocalRotation);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            LocalPosition = br.ReadVector3();
            LocalScale = br.ReadVector3();
            LocalEulerAngles = br.ReadVector3();
            LocalRotation = br.ReadQuaternion();
        }
    }
}