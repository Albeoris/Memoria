using System.IO;

namespace Memoria.Test
{
    internal abstract partial class CommandMessage : IRemotingMessage
    {
        public abstract void Serialize(BinaryWriter bw);
        public abstract void Deserialize(BinaryReader br);
    }
}
