using Memoria.Prime.Ini;
using System;

namespace Memoria
{
	public sealed partial class Configuration
	{
		private sealed class AudioSection : IniSection
		{
			public readonly IniValue<Int32> SoundVolume;
			public readonly IniValue<Int32> MusicVolume;
			public readonly IniValue<Int32> MovieVolume;
			public readonly IniValue<Boolean> PriorityToOGG;

			public AudioSection() : base(nameof(AudioSection), true)
			{
				SoundVolume = BindInt32(nameof(SoundVolume), 100);
				MusicVolume = BindInt32(nameof(MusicVolume), 100);
				MovieVolume = BindInt32(nameof(MovieVolume), 100);
				PriorityToOGG = BindBoolean(nameof(PriorityToOGG), false);
			}
		}
	}
}
