using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOSlider : GOBase
    {
        public readonly UISprite SliderBackground;
        public readonly UISlider Slider;
        public readonly BoxCollider Collider;
        public readonly UIEventListener EventListener;
        public readonly UISprite SliderBar;
        public readonly UISprite SliderThumb;

        public GOSlider(GameObject obj)
            : base(obj)
        {
            SliderBackground = obj.GetComponent<UISprite>();
            Slider = obj.GetComponent<UISlider>();
            Collider = obj.GetComponent<BoxCollider>();
            EventListener = obj.GetComponent<UIEventListener>();
            SliderBar = obj.GetChild(0).GetComponent<UISprite>();
            SliderThumb = obj.GetChild(1).GetComponent<UISprite>();
        }
    }
}
