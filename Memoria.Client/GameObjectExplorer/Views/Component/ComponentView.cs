using Memoria.Test;
using System;
using System.ComponentModel;

namespace Memoria.Client
{
    public class ComponentView<T> : ObjectView<T> where T : ComponentMessage
    {
        public ComponentView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }
    }
}
