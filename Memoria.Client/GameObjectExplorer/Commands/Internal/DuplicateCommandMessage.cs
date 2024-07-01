using Memoria.Client;
using System;
using System.IO;
using System.Windows.Input;

namespace Memoria.Test
{
    internal sealed partial class DuplicateCommandMessage : CommandMessage, ICommand
    {
        public DuplicateCommandMessage(Int32 instanceId)
            : base(CommandMessageType.Duplicate)
        {
            InstanceId = instanceId;
        }

        public void Execute(Object parameter)
        {
            NetworkClient.Execute(this);
        }

        public Boolean CanExecute(Object parameter) => true;

        public event EventHandler CanExecuteChanged;
    }
}
