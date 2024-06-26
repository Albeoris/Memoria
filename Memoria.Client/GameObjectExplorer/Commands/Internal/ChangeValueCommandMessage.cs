﻿using System;
using System.IO;
using System.Windows.Input;
using Memoria.Client;

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