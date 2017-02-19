using UnityEngine;

namespace Memoria.Scenes
{
    internal sealed class GOLocalizableLabel : GOLabel
    {
        public readonly UILocalize Localize;

        public GOLocalizableLabel(GameObject obj)
            : base(obj)
        {
            Localize = obj.GetExactComponent<UILocalize>();
        }
    }
}