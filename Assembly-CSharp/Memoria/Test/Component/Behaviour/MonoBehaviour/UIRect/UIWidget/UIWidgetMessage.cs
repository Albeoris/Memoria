using System;
using System.IO;

namespace Memoria.Test
{
    public class UIWidgetMessage : UIRectMessage
    {
        public Int32 Width;
        public Int32 Height;

        public UIWidgetMessage()
        {
        }

        public UIWidgetMessage(UIWidget uiWidget)
            : base(uiWidget)
        {
            Width = uiWidget.width;
            Height = uiWidget.height;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(Width);
            bw.Write(Height);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            Width = br.ReadInt32();
            Height = br.ReadInt32();
        }
    }
}
