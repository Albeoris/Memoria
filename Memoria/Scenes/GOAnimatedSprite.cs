using System;
using UnityEngine;

namespace Memoria
{
    internal class GOAnimatedSprite : GOSprite
    {
        public readonly UISpriteAnimation SpriteAnimation;

        public GOAnimatedSprite(GameObject obj)
            : base(obj)
        {
            SpriteAnimation = obj.GetExactComponent<UISpriteAnimation>();
        }
    }
}