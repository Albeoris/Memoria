using System;
using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOLabel : GOBase
    {
        public readonly UILabel Label;

        public GOLabel(GameObject obj)
            : base(obj)
        {
            Label = obj.GetExactComponent<UILabel>();
        }

        public void SetText(String value)
        {
            Label.text = value;
        }

        public void SetColor(Color value)
        {
            Label.color = value;
        }
    }
}