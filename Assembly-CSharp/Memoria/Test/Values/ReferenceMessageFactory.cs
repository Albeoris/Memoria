using System;
using System.IO;

namespace Memoria.Test
{
	public static class ReferenceMessageFactory
	{
		public static IReferenceMessage Instantiate(ReferenceMessageType type)
		{
			switch (type)
			{
				case ReferenceMessageType.String:
					return new StringMessage(String.Empty);

				default:
					throw new NotSupportedException(type.ToString());
			}
		}

		public static void Serialize(BinaryWriter bw, IReferenceMessage message)
		{
			bw.Write(message.ReferenceType);
			message.Serialize(bw);
		}

		public static IReferenceMessage Deserialize(BinaryReader br)
		{
			ReferenceMessageType type = br.ReadReferenceMessageType();
			IReferenceMessage value = Instantiate(type);
			value.Deserialize(br);
			return value;
		}
	}
}
