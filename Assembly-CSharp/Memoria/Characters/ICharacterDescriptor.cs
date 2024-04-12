using System;
using UnityEngine;

namespace Memoria
{
	public interface ICharacterDescriptor
	{
		String Name { get; }
		UInt16 Model { get; }
		Int16 Eyes { get; }
		Byte NeckMyId { get; }
		Byte NeckTargetId { get; }
		ObjectFlags ObjectFlags { get; }
		ActorFlags ActorFlags { get; }
		Byte TrackingRadius { get; }
		Byte TalkingRadius { get; }
		Vector3 StartupLocation { get; }
		CharacterAnimation GetAnimation();
	}
}
