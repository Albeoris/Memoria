using System;

namespace Memoria.Client.Interaction
{
    public static class InteractionService
    {
        public static RemoteGameObjectsProviders RemoteGameObjects { get; private set; }
        public static UiGameObjectContent GameObjectsController { get; set; }

        static InteractionService()
        {
            RemoteGameObjects = new RemoteGameObjectsProviders();
        }
    }
}
