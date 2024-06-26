using UnityEngine;

namespace Memoria.Test
{
    public class ScriptableObjectMessage : ObjectMessage
    {
        public const RemotingMessageType Type = RemotingMessageType.ScriptableObject;

        public ScriptableObjectMessage()
        {
        }

        public ScriptableObjectMessage(ScriptableObject scriptableObject)
            : base(scriptableObject)
        {
        }
    }
}