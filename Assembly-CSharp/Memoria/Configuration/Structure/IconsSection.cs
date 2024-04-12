using Memoria.Prime.Ini;
using System;

namespace Memoria
{
	public sealed partial class Configuration
	{
		private sealed class IconsSection : IniSection
		{
			public readonly IniValue<Boolean> HideCursor;
			public readonly IniValue<Boolean> HideExclamation;
			public readonly IniValue<Boolean> HideQuestion;
			public readonly IniValue<Boolean> HideCards;
			public readonly IniValue<Boolean> HideBeach;
			public readonly IniValue<Boolean> HideSteam;

			public IconsSection() : base(nameof(IconsSection), false)
			{
				HideCursor = BindBoolean(nameof(HideCursor), false);
				HideExclamation = BindBoolean(nameof(HideExclamation), false);
				HideQuestion = BindBoolean(nameof(HideQuestion), false);
				HideCards = BindBoolean(nameof(HideCards), false);
				HideBeach = BindBoolean(nameof(HideBeach), false);
				HideSteam = BindBoolean(nameof(HideSteam), false);
			}
		}
	}
}
