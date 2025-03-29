using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOSimpleButton<T> : GOBase where T : GOBase
    {
        public readonly UIButton Button;
        public readonly BoxCollider BoxCollider;
        public readonly UIKeyNavigation KeyNavigation;
        public readonly ButtonGroupState ButtonGroup;
        public readonly T Content;

        public UIEventListener EventListener => UIEventListener.Get(GameObject);

        public GOSimpleButton(GameObject obj)
            : base(obj)
        {
            Button = obj.GetExactComponent<UIButton>();
            BoxCollider = obj.GetExactComponent<BoxCollider>();
            KeyNavigation = obj.GetExactComponent<UIKeyNavigation>();
            ButtonGroup = obj.GetExactComponent<ButtonGroupState>();
            Content = Create<T>(obj);
            if (Configuration.Control.WrapSomeMenus)
                KeyNavigation.wrapUpDown = true;
        }
    }
}
