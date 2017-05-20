using System;

namespace Memoria.Test
{
    public class UIWidgetContainerMessage : MonoBehaviourMessage
    {
        public UIWidgetContainerMessage()
        {
        }

        public UIWidgetContainerMessage(UIWidgetContainer uiWidgetContainer)
            : base(uiWidgetContainer)
        {
        }
    }
}