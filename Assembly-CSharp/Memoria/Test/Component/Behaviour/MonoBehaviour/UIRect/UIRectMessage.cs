using System;
using System.IO;

namespace Memoria.Test
{
    public class UIRectMessage : MonoBehaviourMessage
    {
        public Single Alpha;
        public UIRect.AnchorUpdate UpdateAnchors;
        public UIRect.Position LeftAnchor;
        public UIRect.Position RightAnchor;
        public UIRect.Position BottomAnchor;
        public UIRect.Position TopAnchor;

        public UIRectMessage()
        {
        }

        public UIRectMessage(UIRect uiRect)
            : base(uiRect)
        {
            Alpha = uiRect.alpha;
            UpdateAnchors = uiRect.updateAnchors;
            LeftAnchor = uiRect.LeftAnchorPosition;
            RightAnchor = uiRect.RightAnchorPosition;
            BottomAnchor = uiRect.BottomAnchorPosition;
            TopAnchor = uiRect.TopAnchorPosition;
        }

        public override void Serialize(BinaryWriter bw)
        {
            base.Serialize(bw);

            bw.Write(Alpha);
            bw.Write((Int32)UpdateAnchors);
            LeftAnchor.Write(bw);
            RightAnchor.Write(bw);
            BottomAnchor.Write(bw);
            TopAnchor.Write(bw);
        }

        public override void Deserialize(BinaryReader br)
        {
            base.Deserialize(br);

            Alpha = br.ReadSingle();
            UpdateAnchors = (UIRect.AnchorUpdate)br.ReadInt32();
            LeftAnchor = UIRect.Position.Read(br);
            RightAnchor = UIRect.Position.Read(br);
            BottomAnchor = UIRect.Position.Read(br);
            TopAnchor = UIRect.Position.Read(br);
        }
    }
}
