using Memoria.Prime.Ini;
using System;

namespace Memoria.Assets.Import.Graphics
{
	class PsdInfo : Ini
	{
		#region Static

		public static void Sample()
		{
			PsdInfo ini = PsdInfo.Load("PsdInfo.ini");
			String LayerOrder = ini.LayerOrderFromPsdSection;
			Int32 Reversed = ini.ReversedFromPsdSection;
		}

		public static PsdInfo Load(String filePath)
		{
			PsdInfo result = new PsdInfo();

			IniReader reader = new IniReader(filePath);
			reader.Read(result);

			return result;
		}

		#endregion Static

		private PsdSection _psdSection;

		public String LayerOrderFromPsdSection => _psdSection.LayerOrder.Value;
		public Int32 ReversedFromPsdSection => _psdSection.Reversed.Value;

		public PsdInfo()
		{
			BindingSection(out _psdSection, v => _psdSection = v);
		}

		private sealed class PsdSection : IniSection
		{
			public readonly IniValue<String> LayerOrder;
			public readonly IniValue<Int32> Reversed;

			public PsdSection() : base(nameof(PsdSection), true)
			{
				LayerOrder = BindString(nameof(LayerOrder), "name");
				Reversed = BindInt32(nameof(Reversed), 0);
			}
		}
	}
}
