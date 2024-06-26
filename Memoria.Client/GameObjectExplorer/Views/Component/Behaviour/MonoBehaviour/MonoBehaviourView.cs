using System;
using System.ComponentModel;
using Memoria.Test;

namespace Memoria.Client
{
    public class MonoBehaviourView<T> : ComponentView<T> where T : MonoBehaviourMessage
    {
        public MonoBehaviourView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("MonoBehaviour")]
        [DisplayName("UseGUILayout")]
        [Description("Disabling this lets you skip the GUI layout phase.")]
        public Boolean UseGUILayout
        {
            get { return Native.UseGUILayout; }
            set { Native.UseGUILayout = value; }
        }
    }
}