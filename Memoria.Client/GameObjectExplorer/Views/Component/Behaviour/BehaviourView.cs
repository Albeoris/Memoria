using Memoria.Test;
using System;
using System.ComponentModel;

namespace Memoria.Client
{
	public class BehaviourView<T> : ComponentView<T> where T : BehaviourMessage
	{
		public BehaviourView(T message, RemoteGameObjects context)
			: base(message, context)
		{
		}

		[Category("Behaviour")]
		[DisplayName("IsEnabled")]
		[Description("Enabled Behaviours are Updated, disabled Behaviours are not.")]
		public Boolean LocalPosition
		{
			get { return Native.IsEnabled; }
			set { Native.IsEnabled = value; }
		}
	}
}
