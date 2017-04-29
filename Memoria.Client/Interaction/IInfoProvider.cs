using System.Threading;

namespace Memoria.Client.Interaction
{
    public interface IInfoProvider<out T> where T : class
    {
        string Title { get; }
        string Description { get; }

        T Provide();
    }
}