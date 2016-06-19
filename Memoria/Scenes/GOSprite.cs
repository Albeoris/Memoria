using UnityEngine;

namespace Memoria
{
    internal class GOSprite : GOBase
    {
        public readonly UISprite Sprite;

        public GOSprite(GameObject obj)
            : base(obj)
        {
            Sprite = obj.GetExactComponent<UISprite>();
        }
    }
}