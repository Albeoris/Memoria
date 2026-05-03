using System;
using UnityEngine;

namespace Memoria.Test
{
    public class ComponentMessage : ObjectMessage
    {
        public const RemotingMessageType Type = RemotingMessageType.Component;

        public Int32 ComponentIndex { get; set; }

        public ComponentMessage()
        {
        }

        public ComponentMessage(Component component)
            : base(component)
        {
        }
    }
}
