using System;
using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOButtonPrefab : GOWidget
    {
        public readonly UIButton Button;
        public readonly BoxCollider BoxCollider;
        public readonly UIKeyNavigation KeyNavigation;
        public readonly ButtonGroupState ButtonGroup;
        public readonly ScrollItemKeyNavigation ScrollKeyNavigation;
        public readonly UIDragScrollView DragScrollView;

        // These can be null
        public readonly RecycleListItem ListItem;
        public readonly UILabel NameLabel;
        public readonly UILabel NumberLabel;
        public readonly UISprite IconSprite;
        public readonly UISpriteAnimation IconSpriteAnimation;

        public UIEventListener EventListener => UIEventListener.Get(GameObject);

        public GOButtonPrefab(GameObject obj)
            : base(obj)
        {
            Button = obj.GetExactComponent<UIButton>();
            BoxCollider = obj.GetExactComponent<BoxCollider>();
            KeyNavigation = obj.GetExactComponent<UIKeyNavigation>();
            ButtonGroup = obj.GetExactComponent<ButtonGroupState>();
            ScrollKeyNavigation = obj.GetExactComponent<ScrollItemKeyNavigation>();
            DragScrollView = obj.GetExactComponent<UIDragScrollView>();
            BoxCollider = obj.GetExactComponent<BoxCollider>();
            if (obj.GetComponent<RecycleListItem>() != null)
                ListItem = obj.GetExactComponent<RecycleListItem>();

            GameObject content = obj;
            if (obj.GetChild(0).transform.childCount > 0)
                content = obj.GetChild(0);
            Int32 elementIndex = 0;
            IconSprite = content.GetChild(elementIndex)?.GetComponent<UISprite>();
            IconSpriteAnimation = content.GetChild(elementIndex)?.GetComponent<UISpriteAnimation>();
            if (IconSprite != null)
                elementIndex++;
            NameLabel = content.GetChild(elementIndex++)?.GetComponent<UILabel>();
            NumberLabel = content.GetChild(elementIndex++)?.GetComponent<UILabel>();
        }
    }
}
