using Memoria.Client;
using System;
using System.Windows.Input;

namespace Memoria.Test
{
    internal sealed partial class ChangeReferenceCommandMessage : CommandMessage, ICommand
    {
        public ChangeReferenceCommandMessage(Int32 instanceId, String memberName, IReferenceMessage value)
            : base(CommandMessageType.ChangeReference)
        {
            InstanceId = instanceId;
            MemberName = memberName;
            Value = value;
        }

        public void Execute(Object parameter)
        {
            NetworkClient.Execute(this);
        }

        public Boolean CanExecute(Object parameter) => true;

        public event EventHandler CanExecuteChanged;
    }
}
