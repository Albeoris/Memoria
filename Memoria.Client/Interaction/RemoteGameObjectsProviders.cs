namespace Memoria.Client.Interaction
{
    public sealed class RemoteGameObjectsProviders : InfoProviderGroup<RemoteGameObjects>
    {
        public RemoteGameObjectsProviders()
            : base("Game Objects", "Game object providers")
        {
            Capacity = 1;
            Add(new RemoteGameObjectsProvider());
        }
    }
}
