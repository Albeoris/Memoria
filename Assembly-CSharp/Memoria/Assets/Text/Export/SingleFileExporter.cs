using Memoria.Prime;
using System;
using System.IO;

namespace Memoria.Assets
{
	public abstract class SingleFileExporter : IExporter
	{
		protected abstract String TypeName { get; }
		protected abstract String ExportPath { get; }

		protected abstract TxtEntry[] PrepareEntries();

		public void Export()
		{
			try
			{
				String exportPath = ExportPath;
				if (File.Exists(exportPath))
				{
					Log.Warning($"[{TypeName}] Export was skipped bacause a file already exists: [{exportPath}].");
					return;
				}

				Log.Message($"[{TypeName}] Exporting...");

				TxtEntry[] abilities = PrepareEntries();

				FileCommander.PrepareFileDirectory(exportPath);
				TxtWriter.WriteStrings(exportPath, abilities);

				Log.Message($"[{TypeName}] Exporting completed successfully.");
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"[{TypeName}] Failed to export resource.");
			}
		}
	}
}
