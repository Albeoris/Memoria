using System;
using System.IO;

namespace Memoria.Test
{
    public class UILocalizeMessage : MonoBehaviourMessage
    {
        public String Key;

        public UILocalizeMessage()
        {
        }

        public UILocalizeMessage(UILocalize uiLocalize)
            : base(uiLocalize)
        {
            Key = uiLocalize.key;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(Key);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            Key = br.ReadString();
        }
    }
}