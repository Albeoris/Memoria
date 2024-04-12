using System;

namespace Memoria.Test
{
	public interface IReferenceMessage : IRemotingMessage
	{
		ReferenceMessageType ReferenceType { get; }
		Object Object { get; }
	}
}
