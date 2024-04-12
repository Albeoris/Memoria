using Memoria.Prime.Ini;
using System;

namespace Memoria
{
	public sealed partial class Configuration
	{
		private sealed class SpeedrunSection : IniSection
		{
			public readonly IniValue<String> SplitSettingsPath;
			public readonly IniValue<String> LogGameTimePath;

			public SpeedrunSection() : base(nameof(SpeedrunSection), false)
			{
				SplitSettingsPath = BindPath(nameof(SplitSettingsPath), "");
				LogGameTimePath = BindPath(nameof(LogGameTimePath), "");
			}
		}
	}
}
