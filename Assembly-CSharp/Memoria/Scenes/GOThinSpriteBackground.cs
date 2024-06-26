using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOThinSpriteBackground : GOThinBackground
    {
        public readonly GOSprite Body;

        public GOThinSpriteBackground(GameObject obj) 
            : base(obj)
        {
            Body = new GOSprite(obj.GetChild(1));
        }
    }
}