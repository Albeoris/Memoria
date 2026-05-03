namespace Memoria.Test
{
    internal abstract partial class CommandMessage
    {
        public readonly CommandMessageType MessageType;

        public CommandMessage(CommandMessageType messageType)
        {
            MessageType = messageType;
        }
    }
}
