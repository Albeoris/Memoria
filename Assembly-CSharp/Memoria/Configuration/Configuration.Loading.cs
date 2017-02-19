using System;
using System.IO;
using System.Reflection;
using Memoria.Prime;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration : Ini
    {
        private const String ConfigurationFileName = "Memoria.ini";
        private static readonly Configuration Instance;

        static Configuration()
        {
            Instance = new Configuration();
            LoadSafe();
        }

        private static void LoadSafe()
        {
            try
            {
                EnsureConfigurationFileExists();
                IniReader reader = new IniReader(ConfigurationFileName);
                reader.Read(Instance);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load a configuration.");
            }
        }

        private static void EnsureConfigurationFileExists()
        {
            if (File.Exists(ConfigurationFileName))
                return;

            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Memoria.Configuration." + ConfigurationFileName))
            using (Stream output = File.Create(ConfigurationFileName))
                input.CopyTo(output, 8192);
        }
    }
}