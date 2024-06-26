using Memoria.Test;

namespace Memoria.Client
{
    public class ScriptableObjectView<T> : ObjectView<T> where T : ScriptableObjectMessage
    {
        public ScriptableObjectView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }
    }
}
