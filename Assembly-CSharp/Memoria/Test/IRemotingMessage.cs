using System.IO;

namespace Memoria.Test
{
    public partial interface IRemotingMessage
    {
        void Serialize(BinaryWriter bw);
        void Deserialize(BinaryReader br);
    }
}