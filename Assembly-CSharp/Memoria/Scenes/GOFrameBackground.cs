using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOFrameBackground : GOWidget
    {
        public readonly GOLocalizableLabel Caption;
        public readonly GOSprite Border;
        public readonly GOSprite Body;
        public readonly GOSprite Shadow;

        public GOFrameBackground(GameObject obj)
            : base(obj)
        {
            // Not always in the same order
            Caption = new GOLocalizableLabel(obj.FindChild("Caption"));
            Body = new GOSprite(obj.FindChild("Body"));
            Border = new GOSprite(obj.FindChild("Border"));
            Shadow = new GOSprite(obj.FindChild("Shadow"));
            Caption.Label.preventWrapping = true;
            Caption.Label.overflowMethod = UILabel.Overflow.ClampContent;
        }
    }
}
