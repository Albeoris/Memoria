using Memoria.Client;
using System;
using System.Windows.Input;

namespace Memoria.Test
{
	internal sealed partial class ChangeValueCommandMessage : CommandMessage, ICommand
	{
		public ChangeValueCommandMessage(Int32 instanceId, String memberName, IValueMessage value)
			: base(CommandMessageType.ChangeValue)
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
