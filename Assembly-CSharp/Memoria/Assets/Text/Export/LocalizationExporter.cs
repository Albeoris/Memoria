using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
	public sealed class LocalizationExporter : SingleFileExporter
	{
		protected override String TypeName => nameof(LocalizationExporter);
		protected override String ExportPath => ModTextResources.Export.System;

		protected override TxtEntry[] PrepareEntries()
		{
			String symbol = EmbadedTextResources.CurrentSymbol ?? Localization.GetSymbol();

			SortedList<String, String> dic = Localization.Provider.ProvideDictionary(symbol);
			IList<String> keys = dic.Keys;
			IList<String> values = dic.Values;

			TxtEntry[] result = new TxtEntry[dic.Count];
			for (Int32 i = 0; i < result.Length; i++)
				result[i] = new TxtEntry { Prefix = keys[i], Value = values[i] };

			return result;
		}
	}
}
