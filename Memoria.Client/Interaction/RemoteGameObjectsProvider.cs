using System;

namespace Memoria.Client.Interaction
{
	public sealed class RemoteGameObjectsProvider : IInfoProvider<RemoteGameObjects>
	{
		public String Title => "Default";
		public String Description => "Provider game objects via network connection.";

		public RemoteGameObjects Provide()
		{
			return new RemoteGameObjects();
		}
	}
}
