using System;
using System.ComponentModel;
using Memoria.Test;

namespace Memoria.Client
{
    public class UILocalizeView<T> : MonoBehaviourView<T> where T : UILocalizeMessage
    {
        public UILocalizeView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("UILocalize")]
        [DisplayName("Key")]
        [Description("Unknown")]
        public String Key
        {
            get { return Native.Key; }
            set { Native.Key = value; }
        }
    }
}