using Memoria.Test;

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
