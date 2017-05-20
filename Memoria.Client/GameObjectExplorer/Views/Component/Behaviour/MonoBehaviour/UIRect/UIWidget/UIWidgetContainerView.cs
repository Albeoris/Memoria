using Memoria.Test;

namespace Memoria.Client
{
    public class UIWidgetContainerView<T> : MonoBehaviourView<T> where T : UIWidgetContainerMessage
    {
        public UIWidgetContainerView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }
    }
}