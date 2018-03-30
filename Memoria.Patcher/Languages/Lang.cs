using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Memoria.Patcher
{
    public sealed partial class Lang
    {
        #region Lazy

        private static readonly Lazy<Lang> Instance = new Lazy<Lang>(Initialize, true);

        private static Lang Initialize()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                XmlElement def = XmlHelper.LoadEmbadedDocument(assembly, $"Memoria.Patcher.Languages.en.xml");
                XmlElement cur = null;

                String[] fileNames = {CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName};
                foreach (String name in fileNames)
                {
                    cur = XmlHelper.LoadEmbadedDocument(assembly, $"Memoria.Patcher.Languages.{name}.xml");
                    if (cur != null)
                        break;
                }

                return new Lang(def, cur);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize Lang Data.");
                Console.WriteLine(ex);
                Console.ReadLine();
                Environment.Exit(1);
            }
            return null;
        }

        #endregion

        #region Instance

        private readonly XmlElement _default;
        private readonly XmlElement _current;

        private Lang(XmlElement def, XmlElement cur)
        {
            _default = def ?? throw new ArgumentNullException(nameof(def));
            _current = cur ?? throw new ArgumentNullException(nameof(cur));
        }

        public string GetString(string arg, params string[] path)
        {
            if (arg == null) throw new ArgumentNullException(nameof(arg));
            if (arg == String.Empty) throw new ArgumentException(nameof(arg));

            try
            {
                string str = FindString(_current, arg, path);
                if (str != null)
                    return str;
                Console.WriteLine("String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
                Console.WriteLine(ex);
            }

            try
            {
                string str = FindString(_default, arg, path);
                if (str != null)
                    return str;
                Console.WriteLine("String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("String is not found in default dictionary: {0}.{1}", string.Join(".", path), arg);
                Console.WriteLine(ex);
            }

            return "#StringNotFound#";
        }

        private string FindString(XmlElement root, string arg, params string[] path)
        {
            XmlElement result = root;
            foreach (string s in path)
            {
                result = result[s];
                if (result == null)
                    break;
            }

            return result?.FindString(arg);
        }

        #endregion
    }

    public sealed partial class Lang
    {
        public static class Measurement
        {
            private static string GetMeasurement(string name)
            {
                return Instance.Value.GetString(name, "Measurement");
            }

            public static readonly string Elapsed = GetMeasurement("Elapsed");
            public static readonly string Remaining = GetMeasurement("Remaining");
            public static readonly string SecondAbbr = GetMeasurement("SecondAbbr");
            public static readonly string ByteAbbr = GetMeasurement("ByteAbbr");
            public static readonly string KByteAbbr = GetMeasurement("KByteAbbr");
            public static readonly string MByteAbbr = GetMeasurement("MByteAbbr");
            public static readonly string GByteAbbr = GetMeasurement("GByteAbbr");
            public static readonly string TByteAbbr = GetMeasurement("TByteAbbr");
            public static readonly string PByteAbbr = GetMeasurement("PByteAbbr");
            public static readonly string EByteAbbr = GetMeasurement("EByteAbbr");
        }

        public static class Button
        {
            private static string GetButton(string name)
            {
                return Instance.Value.GetString(name, "Button");
            }

            public static readonly string OK = GetButton("OK");
            public static readonly string Cancel = GetButton("Cancel");
            public static readonly string Continue = GetButton("Continue");
            public static readonly string Clone = GetButton("Clone");
            public static readonly string Remove = GetButton("Remove");
            public static readonly string Rollback = GetButton("Rollback");
            public static readonly string Inject = GetButton("Inject");
            public static readonly string SaveAs = GetButton("SaveAs");
        }

        public static class InfoProvider
        {
            private static string GetInfoProvider(string name, string providerName)
            {
                return Instance.Value.GetString(name, "InfoProvider", providerName);
            }

            public static class ApplicationConfig
            {
                private static string GetApplicationConfig(string name)
                {
                    return GetInfoProvider(name, "ApplicationConfig");
                }

                public static readonly string Title = GetApplicationConfig("Title");
                public static readonly string Description = GetApplicationConfig("Description");
                public static readonly string NewTitle = GetApplicationConfig("NewTitle");
                public static readonly string NewDescription = GetApplicationConfig("NewDescription");
                public static readonly string FileTitle = GetApplicationConfig("FileTitle");
                public static readonly string FileDescription = GetApplicationConfig("FileDescription");
            }

            public static class GameLocation
            {
                private static string GetGameLocation(string name)
                {
                    return GetInfoProvider(name, "GameLocation");
                }

                public static readonly string Title = GetGameLocation("Title");
                public static readonly string Description = GetGameLocation("Description");
                public static readonly string ConfigurationTitle = GetGameLocation("ConfigurationTitle");
                public static readonly string ConfigurationDescription = GetGameLocation("ConfigurationDescription");
                public static readonly string SteamRegistryTitle = GetGameLocation("SteamRegistryTitle");
                public static readonly string SteamRegistryDescription = GetGameLocation("SteamRegistryDescription");
                public static readonly string UserTitle = GetGameLocation("UserTitle");
                public static readonly string UserDescription = GetGameLocation("UserDescription");
            }

            public static class WorkingLocation
            {

                private static string GetWorkingLocation(string name)
                {
                    return GetInfoProvider(name, "WorkingLocation");
                }

                public static readonly string Title = GetWorkingLocation("Title");
                public static readonly string Description = GetWorkingLocation("Description");
                public static readonly string ConfigurationTitle = GetWorkingLocation("ConfigurationTitle");
                public static readonly string ConfigurationDescription = GetWorkingLocation("ConfigurationDescription");
                public static readonly string UserTitle = GetWorkingLocation("UserTitle");
                public static readonly string UserDescription = GetWorkingLocation("UserDescription");
            }

            public static class TextEncoding
            {
                private static string GetTextEncoding(string name)
                {
                    return GetInfoProvider(name, "TextEncoding");
                }

                public static readonly string Title = GetTextEncoding("Title");
                public static readonly string Description = GetTextEncoding("Description");
                public static readonly string NewTitle = GetTextEncoding("NewTitle");
                public static readonly string NewDescription = GetTextEncoding("NewDescription");
                public static readonly string WorkingLocationTitle = GetTextEncoding("WorkingLocationTitle");
                public static readonly string WorkingLocationDescription = GetTextEncoding("WorkingLocationDescription");
                public static readonly string UserTitle = GetTextEncoding("UserTitle");
                public static readonly string UserDescription = GetTextEncoding("UserDescription");
            }

            public static class AudioSettings
            {
                private static string GetAudioSettings(string name)
                {
                    return GetInfoProvider(name, "AudioSettings");
                }

                public static readonly string Title = GetAudioSettings("Title");
                public static readonly string Description = GetAudioSettings("Description");
                public static readonly string NewTitle = GetAudioSettings("NewTitle");
                public static readonly string NewDescription = GetAudioSettings("NewDescription");
            }
        }

        public static class Dockable
        {
            private static string GetDockable(string name, string dockableName)
            {
                return Instance.Value.GetString(name, "Dockable", dockableName);
            }

            private static string GetDockable(string name, string dockableName, string childName)
            {
                return Instance.Value.GetString(name, "Dockable", dockableName, childName);
            }

            public static class DataSources
            {
                private static string GetDockableInfoProviders(string name)
                {
                    return GetDockable(name, "DataSources");
                }

                public static readonly string Header = GetDockableInfoProviders("Header");
            }

            public static class GameFileCommander
            {
                private static string GetDockableInfoProviders(string name)
                {
                    return GetDockable(name, "GameFileCommander");
                }

                public static readonly string Header = GetDockableInfoProviders("Header");
                public static readonly string Unpack = GetDockableInfoProviders("Unpack");
                public static readonly string Pack = GetDockableInfoProviders("Pack");
                public static readonly string ArchivesNode = GetDockableInfoProviders("ArchivesNode");
            }

            public static class GameFilePreview
            {
                private static string GetDockableInfoProviders(string name)
                {
                    return GetDockable(name, "GameFilePreview");
                }

                private static string GetDockableInfoProviders(string name, string childName)
                {
                    return GetDockable(name, "GameFilePreview", childName);
                }

                public static readonly string Header = GetDockableInfoProviders("Header");

                public static class Ykd
                {
                    private static string GetDockableInfoProvidersYkd(string name)
                    {
                        return GetDockableInfoProviders(name, "Ykd");
                    }

                    public static readonly string ResourceRemovingTitle = GetDockableInfoProvidersYkd("ResourceRemovingTitle");
                    public static readonly string ConfirmResourceRemoving = GetDockableInfoProvidersYkd("ConfirmResourceRemoving");
                }
            }
        }

        public static class EncodingEditor
        {
            private static string GetEncodingEditor(string name, string childName)
            {
                return Instance.Value.GetString(name, nameof(EncodingEditor), childName);
            }
            
            public static class List
            {
                private static string GetEncodingEditorList(string name)
                {
                    return GetEncodingEditor(name, nameof(List));
                }

                public static readonly string Name = GetEncodingEditorList(nameof(Name));
                public static readonly string Pattern = GetEncodingEditorList(nameof(Pattern));
                public static readonly string Encoding = GetEncodingEditorList(nameof(Encoding));
                public static readonly string Font = GetEncodingEditorList(nameof(Font));
                public static readonly string Edit = GetEncodingEditorList(nameof(Edit));
            }

            public static class EncodingEditDialog
            {
                private static string GetEncodingEditorEncodingEditDialog(string name)
                {
                    return GetEncodingEditor(name, nameof(EncodingEditDialog));
                }

                public static readonly string Title = GetEncodingEditorEncodingEditDialog(nameof(Title));
            }

            public static class EncodingSelectDialog
            {
                private static string GetEncodingEditorEncodingSelectDialog(string name)
                {
                    return GetEncodingEditor(name, nameof(EncodingSelectDialog));
                }

                public static readonly string Title = GetEncodingEditorEncodingSelectDialog(nameof(Title));
                public static readonly string Watermark = GetEncodingEditorEncodingSelectDialog(nameof(Watermark));
                public static readonly string ConfirmMessageFormat = GetEncodingEditorEncodingSelectDialog(nameof(ConfirmMessageFormat));
                public static readonly string ErrorMessageFormat = GetEncodingEditorEncodingSelectDialog(nameof(ErrorMessageFormat));
            }

            public static class Main
            {
                private static string GetEncodingEditorMain(string name)
                {
                    return GetEncodingEditor(name, "Main");
                }

                public static readonly string Before = GetEncodingEditorMain("Before");
                public static readonly string Width = GetEncodingEditorMain("Width");
                public static readonly string After = GetEncodingEditorMain("After");
                public static readonly string FromText = GetEncodingEditorMain("FromText");
                public static readonly string ToText = GetEncodingEditorMain("ToText");
            }

            public static class Extra
            {
                private static string GetEncodingEditorExtra(string name)
                {
                    return GetEncodingEditor(name, "Extra");
                }

                public static readonly string Row = GetEncodingEditorExtra("Row");
                public static readonly string Column = GetEncodingEditorExtra("Column");
                public static readonly string FromText = GetEncodingEditorExtra("FromText");
                public static readonly string ToText = GetEncodingEditorExtra("ToText");
            }
        }

        public static class Dialogue
        {
            private static string GetDialogue(string name, string attrName)
            {
                return Instance.Value.GetString(name, "Dialogue", attrName);
            }

            public static class SaveAs
            {
                private static string GetDialogueSaveAs(string name)
                {
                    return GetDialogue(name, "SaveAs");
                }

                public static readonly string Title = GetDialogueSaveAs("Title");
            }
        }

        public static class Message
        {
            private static string GetMessage(string name, string attrName)
            {
                return Instance.Value.GetString(name, "Message", attrName);
            }

            public static class Done
            {
                private static string GetMessageDone(string name)
                {
                    return GetMessage(name, "Done");
                }

                public static readonly string Title = GetMessageDone("Title");
                public static readonly string ExtractionCompleteFormat = GetMessageDone("ExtractionCompleteFormat");
                public static readonly string InjectionCompleteFormat = GetMessageDone("InjectionCompleteFormat");
                public static readonly string Success = GetMessageDone(nameof(Success));
                public static readonly string PressEnterToExit = GetMessageDone(nameof(PressEnterToExit));
            }

            public static class Question
            {
                private static string GetMessageQuestion(string name)
                {
                    return GetMessage(name, nameof(Question));
                }

                public static readonly string AreYouSureTitle = GetMessageQuestion(nameof(AreYouSureTitle));
            }

            public static class Error
            {
                private static string GetMessageError(string name)
                {
                    return GetMessage(name, "Error");
                }

                public static readonly string Title = GetMessageError("Title");
            }
        }

        public static class Error
        {
            private static string GetError(string name, string dockableName)
            {
                return Instance.Value.GetString(name, "Error", dockableName);
            }

            public static class File
            {
                private static string GetErrorFile(string name)
                {
                    return GetError(name, "File");
                }

                public static readonly string UnknownFormat = GetErrorFile("UnknownFormat");
            }

            public static class Process
            {
                private static string GetErrorProcess(string name)
                {
                    return GetError(name, "Process");
                }

                public static readonly string CannotGetExecutablePath = GetErrorProcess("CannotGetExecutablePath");
            }

            public static class Text
            {
                private static string GetErrorText(string name)
                {
                    return GetError(name, "Text");
                }

                public static readonly string TooLongTagNameFormat = GetErrorText("TooLongTagNameFormat");
            }
        }
    }
}