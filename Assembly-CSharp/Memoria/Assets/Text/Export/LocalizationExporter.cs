using System;
using System.Collections.Generic;
using System.Linq;

namespace Memoria.Assets
{
    public sealed class LocalizationExporter : SingleFileExporter
    {
        protected override String TypeName => nameof(LocalizationExporter);
        protected override String ExportPath => ModTextResources.Export.System;

        protected override TxtEntry[] PrepareEntries()
        {
            Int32 ind = Localization.Provider.CurrentLanguageIndex;
            Dictionary<String, String[]> dic = Localization.Provider.ProvideDictionary();
            return dic.Select(p => new TxtEntry {Prefix = p.Key, Value = p.Value[ind]}).ToArray();
        }
    }
}