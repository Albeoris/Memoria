using Memoria.Prime;
using Memoria.Prime.Ini;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Memoria
{
	public sealed partial class Configuration : Ini
	{
		private const String ConfigurationFileName = "Memoria.ini";
		private static readonly Configuration Instance;
		private static readonly FileSystemWatcher Watcher;
		private static volatile Boolean s_changed;

		static Configuration()
		{
			Instance = new Configuration();
			Watcher = CreateWatcher();
			LoadSafe();
		}

		private static FileSystemWatcher CreateWatcher()
		{
			FileSystemWatcher watcher = new FileSystemWatcher();
			GameLoopManager.Quit += watcher.Dispose;
			watcher.Filter = ConfigurationFileName;

			watcher.IncludeSubdirectories = false;
			watcher.Changed += OnChanged;
			watcher.Created += OnChanged;
			return watcher;
		}

		public static void SaveValue<T>(String sectionName, IniValue<T> value)
		{
			if (!Monitor.TryEnter(Watcher))
				return;

			try
			{
				sectionName = $"[{sectionName}]";

				String[] lines = File.ReadAllLines(ConfigurationFileName);
				Int32 startSection = Array.IndexOf(lines, sectionName);
				if (startSection < 0)
				{
					AppendSection(sectionName, value, lines);
					return;
				}

				Int32 index;
				for (index = startSection + 1; index < lines.Length; index++)
				{
					var line = lines[index];
					if (line == String.Empty || line[0] == '[')
						break;

					if (line.StartsWith(value.Name))
					{
						ChangeValue(value, index, lines);
						return;
					}
				}

				AppendValue(value, index, lines);
			}
			catch (Exception err)
			{
				Log.Error(err, $"Failed to update a configuration value {sectionName} {value.Name}.");
			}
			finally
			{
				Monitor.Exit(Watcher);
			}
		}

		private static void AppendSection<T>(String sectionName, IniValue<T> value, String[] lines)
		{
			using (var output = File.AppendText(ConfigurationFileName))
			{
				if (lines.Length != 0 && lines.Last() != String.Empty)
				{
					output.WriteLine();
					output.WriteLine(sectionName);
				}

				value.WriteLine(output);
			}
		}

		private static void AppendValue<T>(IniValue<T> value, Int32 index, String[] lines)
		{
			using (var output = File.CreateText(ConfigurationFileName))
			{
				// Copy previous
				for (int i = 0; i < index; i++)
					output.WriteLine(lines[i]);

				value.WriteLine(output);

				for (int i = index; i < lines.Length; i++)
					output.WriteLine(lines[i]);
			}
		}

		private static void ChangeValue<T>(IniValue<T> value, Int32 index, String[] lines)
		{
			using (var output = File.CreateText(ConfigurationFileName))
			{
				// Copy previous
				for (int i = 0; i < index; i++)
					output.WriteLine(lines[i]);

				value.WriteLine(output);

				for (int i = index + 1; i < lines.Length; i++)
					output.WriteLine(lines[i]);
			}
		}

		private static void LoadSafe()
		{
			try
			{
				EnsureConfigurationFileExists();
				IniReader reader = new IniReader(ConfigurationFileName);
				reader.Read(Instance);
				Watcher.EnableRaisingEvents = true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Failed to load a configuration.");
			}
		}

		private static void OnChanged(Object sender, FileSystemEventArgs e)
		{
			try
			{
				s_changed = true;

				if (!Monitor.TryEnter(Watcher))
					return;

				try
				{
					do
					{
						s_changed = false;

						IniReader reader = new IniReader(ConfigurationFileName);
						reader.Read(Instance);
					} while (s_changed);
				}
				finally
				{
					Monitor.Exit(Watcher);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Failed to refresh a configuration.");
			}
		}

		private static void EnsureConfigurationFileExists()
		{
			if (File.Exists(ConfigurationFileName))
				return;

			using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Assembly-CSharp.Memoria.Configuration." + ConfigurationFileName))
			using (Stream output = File.Create(ConfigurationFileName))
				input.CopyTo(output, 8192);
		}
	}
}
