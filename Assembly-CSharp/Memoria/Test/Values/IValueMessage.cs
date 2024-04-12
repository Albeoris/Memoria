using System;

namespace Memoria.Test
{
	public interface IValueMessage : IRemotingMessage
	{
		ValueMessageType ValueType { get; }
		ValueType Object { get; }
	}
}
