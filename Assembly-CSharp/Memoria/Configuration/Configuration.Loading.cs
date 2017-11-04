using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Memoria.Prime;
using Memoria.Prime.Ini;

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
            Watcher = CreareWatcher();
            LoadSafe();
        }

        private static FileSystemWatcher CreareWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            GameLoopManager.Quit += watcher.Dispose;
            watcher.Filter = ConfigurationFileName;

            watcher.IncludeSubdirectories = false;
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            return watcher;
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