using Assets.Sources.Scripts.UI.Common;
using Memoria.Prime;
using Memoria.Prime.Text;
using Memoria.Prime.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using ExtensionMethodsIEnumerable = Memoria.Scenes.ExtensionMethodsIEnumerable;

namespace Memoria.Assets
{
    public sealed class FieldImporter : TextImporter
    {
        private const String EndingTag = "[ENDN]";
        private const Int32 EndingLength = 6;

        private static String TypeName => nameof(FieldImporter);
        private readonly ImportFieldTags _formatter = new ImportFieldTags();
        private volatile Dictionary<Int32, String[]> _cache = new Dictionary<Int32, String[]>();

        private volatile Boolean _initialized;
        private volatile FileSystemWatcher _watcher;
        private volatile AutoResetEvent _watcherEvent;
        private volatile Task _watcherTask;
        private Int32 _fieldZoneId;
        private String _fieldFileName;
        private String[] _original;
        private TxtEntry[] _external;
        private Dictionary<String, LinkedList<UInt64>> _dic;
        private IList<KeyValuePair<String, TextReplacement>> _customTags;
        private IList<KeyValuePair<String, TextReplacement>> _fieldReplacements;
        private Dictionary<String, IList<KeyValuePair<String, TextReplacement>>> _fieldTags;

        public Task InitializationTask { get; private set; }

        public void InitializeAsync()
        {
            InitializationTask = Task.Run(Initialize);
        }

        private void Initialize()
        {
            String directory = ModTextResources.Import.FieldsDirectory;
            if (!Directory.Exists(directory))
            {
                Log.Warning($"[{TypeName}] Import was skipped because a directory does not exist: [{directory}].");
                return;
            }

            Log.Message($"[{TypeName}] Initialization...");

            try
            {
                _dic = new Dictionary<String, LinkedList<UInt64>>();
                _customTags = LoadCustomTags();
                _fieldTags = LoadFieldTags();

                KeyValuePair<Int32, String> chocoboForest = new KeyValuePair<Int32, String>(945, "MES_CHOCO");
                foreach (KeyValuePair<Int32, String> pair in ExtensionMethodsIEnumerable.Append(FF9DBAll.EventDB, chocoboForest))
                {
                    _fieldZoneId = pair.Key;
                    _fieldFileName = _fieldZoneId.ToString("D4", CultureInfo.InvariantCulture) + '_' + pair.Value;

                    if (!ReadEmbeddedText(_fieldZoneId, out _original))
                    {
                        _cache[_fieldZoneId] = null;
                        continue;
                    }

                    if (!ReadExternalText(out _external))
                    {
                        Log.Warning($"[{TypeName}] External file not found: [{Path.Combine(ModTextResources.Import.FieldsDirectory, _fieldFileName)}]");
                        _cache[_fieldZoneId] = _original;
                        continue;
                    }

                    if (_original.Length != _external.Length)
                    {
                        Log.Warning($"[{TypeName}] Number of lines in files does not match. [Original: {_original.Length}, External: {_external.Length}]");
                        _cache[_fieldZoneId] = _original;
                        continue;
                    }

                    _fieldTags.TryGetValue(_fieldFileName, out _fieldReplacements);
                    _cache[_fieldZoneId] = MergeEntries();
                }

                ResolveReferences();

                _initialized = true;

                if (_watcher == null)
                {
                    _watcherEvent = new AutoResetEvent(false);
                    _watcherTask = Task.Run(DoWatch);

                    _watcher = new FileSystemWatcher(directory, "*.*");
                    GameLoopManager.Quit += _watcher.Dispose;

                    _watcher.Changed += OnChangedFileInDirectory;
                    _watcher.Created += OnChangedFileInDirectory;
                    _watcher.Deleted += OnChangedFileInDirectory;

                    _watcher.EnableRaisingEvents = true;
                }

                Log.Message($"[{TypeName}] Initialization completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Initialization failed.");
            }
            finally
            {
                _dic = null;
                _original = null;
                _external = null;
                _fieldReplacements = null;
                _fieldZoneId = -1;
            }
        }

        private void DoWatch()
        {
            Int32 retryCount = 20;

            try
            {
                while (retryCount > 0)
                {
                    try
                    {
                        _watcherEvent.WaitOne();

                        DateTime beginTime = DateTime.UtcNow;
                        Log.Message("[TextWatcher] Field's text was changed. Reinitialize...");

                        FieldImporter importer = new FieldImporter();
                        importer._watcher = _watcher;
                        importer.Initialize();
                        _cache = importer._cache;
                        _customTags = importer._customTags;
                        _fieldTags = importer._fieldTags;

                        importer = null;

                        LoadExternal();

                        Log.Message($"[TextWatcher] Reinitialized for {DateTime.UtcNow - beginTime}");
                    }
                    catch (Exception ex)
                    {
                        retryCount--;
                        Log.Error(ex, "[TextWatcher] Failed to iterate.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[TextWatcher] Failed to watch.");
            }
        }

        private void OnChangedFileInDirectory(Object sender, FileSystemEventArgs e)
        {
            String extension = Path.GetExtension(e.Name);
            if (TextResourceFormatHelper.TryResolveFileFormat(extension, out _))
                _watcherEvent.Set();
        }

        private static IList<KeyValuePair<String, TextReplacement>> LoadCustomTags()
        {
            TextResourceReference inputReference = ModTextResources.Import.FieldTags;
            return ReadTagReplacements(inputReference);
        }

        private static Dictionary<String, IList<KeyValuePair<String, TextReplacement>>> LoadFieldTags()
        {
            String fieldsDirectory = ModTextResources.Import.FieldsDirectory;
            if (!Directory.Exists(fieldsDirectory))
                return new Dictionary<String, IList<KeyValuePair<String, TextReplacement>>>(0);

            String[] filePaths = Directory.GetFiles(fieldsDirectory, "*_Tags.*");
            Dictionary<String, IList<KeyValuePair<String, TextReplacement>>> fieldNames = new(filePaths.Length);
            foreach (String path in filePaths)
            {
                String fileName = Path.GetFileName(path); // 0074_EVT_BATTLE_SIOTES01_Tags.strings
                String extension = Path.GetExtension(fileName); // .strings

                fileName = fileName.Substring(0, fileName.Length - "_Tags".Length - extension.Length); // 0074_EVT_BATTLE_SIOTES01

                TextResourcePath inputPath = TextResourcePath.ForImportExistingFile(path);
                fieldNames[fileName] = ReadTagReplacements(inputPath);
            }

            return fieldNames;
        }

        private static IList<KeyValuePair<String, TextReplacement>> ReadTagReplacements(TextResourceReference inputReference)
        {
            if (!inputReference.IsExists(out TextResourcePath existingFile))
                return new List<KeyValuePair<String, TextReplacement>>();

            return ReadTagReplacements(existingFile);
        }

        private static IList<KeyValuePair<String, TextReplacement>> ReadTagReplacements(TextResourcePath existingFile)
        {
            TxtEntry[] generalNames = existingFile.ReadAll();
            if (generalNames.IsNullOrEmpty())
                return new List<KeyValuePair<String, TextReplacement>>();

            TextReplacements result = new(generalNames.Length);
            foreach (TxtEntry entry in generalNames)
                result.Add(entry.Prefix, entry.Value);

            return result.Forward;
        }

        private void ResolveReferences()
        {
            Int32 currentZoneId = -1;
            String[] currentText = null;
            foreach (KeyValuePair<String, LinkedList<UInt64>> pair in _dic)
            {
                Int32 zoneId, lineId;
                String target = pair.Key;
                do
                {
                    String str = target.Substring(2, 4);
                    if (!Int32.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out zoneId))
                    {
                        Log.Warning($"[{TypeName}] Failed to parse a zone ID from the reference: [{str}].");
                        break;
                    }

                    str = target.Substring(target.Length - 5, 4);
                    if (!Int32.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out lineId))
                    {
                        Log.Warning($"[{TypeName}] Failed to parse a line number from the reference: [{str}].");
                        break;
                    }

                    if (zoneId != currentZoneId)
                    {
                        if (!_cache.TryGetValue(zoneId, out currentText))
                        {
                            Log.Warning($"[{TypeName}] Failed to find zone by ID: {zoneId}].");
                            break;
                        }

                        currentZoneId = zoneId;
                    }

                    if (currentText == null)
                    {
                        Log.Warning($"[{TypeName}] Failed to find text by zone ID: {zoneId}].");
                        break;
                    }

                    if (lineId > currentText.Length)
                    {
                        Log.Warning($"[{TypeName}] Failed to find line by number: {lineId}].");
                        break;
                    }

                    target = currentText[lineId];

                    // new
                    String currentZoneName = GetZoneName(currentZoneId);
                    IList<KeyValuePair<String, TextReplacement>> tags;
                    if (_fieldTags.TryGetValue(currentZoneName, out tags))
                        target = target.ReplaceAll(tags);

                } while (target.StartsWith("{$"));

                foreach (UInt64 value in pair.Value)
                {
                    zoneId = (Int32)(value >> 32);
                    lineId = (Int32)(value & UInt32.MaxValue);

                    _cache[zoneId][lineId] = target;
                }
            }
        }

        private String GetZoneName(Int32 zoneId)
        {
            String name;
            if (zoneId == 945)
                name = "MES_CHOCO";
            else
                name = FF9DBAll.EventDB[zoneId];

            return zoneId.ToString("D4", CultureInfo.InvariantCulture) + '_' + name;
        }

        private String[] MergeEntries()
        {
            String[] result = new String[_original.Length];
            for (Int32 index = 0; index < result.Length; index++)
            {
                String original = _original[index];
                if (String.IsNullOrEmpty(original))
                {
                    result[index] = original;
                    continue;
                }

                String external = _external[index].Value;
                if (external.StartsWith("{$"))
                {
                    LinkedList<UInt64> list;
                    if (!_dic.TryGetValue(external, out list))
                    {
                        list = new LinkedList<UInt64>();
                        _dic[external] = list;
                    }
                    list.AddLast(((UInt64)_fieldZoneId << 32) | (UInt32)index);
                    result[index] = external;
                    continue;
                }

                //external = external.ReplaceAll(_fieldReplacements, _customTags, _formatter.SimpleTags.Backward, _formatter.ComplexTags).TrimEnd();
                //external = external.ReplaceAll(_fieldReplacements, _customTags).TrimEnd();
                external = external.TrimEnd();
                String ending = GetEnding(original);
                if (ending != String.Empty)
                    external += ending;

                result[index] = external;
            }
            return result;
        }

        public static String GetEnding(String str)
        {
            StringBuilder sb = new StringBuilder(6);
            for (Int32 i = str.Length - 1; i >= 0; i--)
            {
                Char ch = str[i];
                if (ch == '\n' || ch == ' ')
                {
                    sb.Insert(0, ch);
                    continue;
                }

                Int32 offset = i - EndingLength + 1;
                if (offset < 0)
                    continue;

                if (str.IndexOf(EndingTag, offset, EndingLength, StringComparison.Ordinal) != offset)
                    break;

                sb.Insert(0, EndingTag);
                i = offset;
            }

            return sb.Length > 0 ? sb.ToString() : String.Empty;
        }

        private Boolean ReadExternalText(out TxtEntry[] entries)
        {
            TextResourceReference inputReference = new(Path.Combine(ModTextResources.Import.FieldsDirectory, _fieldFileName));
            if (!inputReference.IsExists(out TextResourcePath existingFile))
            {
                entries = null;
                return false;
            }

            entries = existingFile.ReadAll();
            return !entries.IsNullOrEmpty();
        }

        protected override Boolean LoadInternal()
        {
            Int32 fieldZoneId = FF9TextTool.FieldZoneId;
            String fieldLanguage = EmbadedTextResources.CurrentSymbol ?? Localization.CurrentSymbol;
            if (fieldZoneId == FF9TextTool.LoadingZoneBatch.fieldZoneId && fieldLanguage == FF9TextTool.LoadingZoneBatch.fieldLangSymbol)
                return true;

            FF9TextTool.LoadingZoneBatch.fieldText.Clear();
            String path = EmbadedTextResources.GetCurrentPath("/Field/" + FF9TextTool.GetFieldTextFileName(fieldZoneId) + ".mes");
            FF9TextTool.ImportStrtWithCumulativeModFiles<Int32>(path, FF9TextTool.LoadingZoneBatch.fieldText);

            FF9TextTool.ClearTableText();

            if (FF9TextTool.LoadingZoneBatch.fieldText.Count == 0)
                return false;
            FF9TextTool.LoadingZoneBatch.UpdateFieldZone(fieldZoneId, fieldLanguage);
            return true;
        }

        private static Boolean ReadEmbeddedText(Int32 fieldZoneId, out String[] text)
        {
            String path = EmbadedTextResources.GetCurrentPath("/Field/" + FF9TextTool.GetFieldTextFileName(fieldZoneId) + ".mes");
            String raw = EmbadedSentenseLoader.LoadText(path);

            if (raw != null)
            {
                text = FF9TextTool.ExtractSentense(new Dictionary<Int32, String>(), raw).Values.ToArray();
                return true;
            }

            text = null;
            return false;
        }

        protected override Boolean LoadExternal()
        {
            try
            {
                if (!_initialized)
                    return false;

                Int32 fieldZoneId = FF9TextTool.FieldZoneId;

                String[] result;
                if (!_cache.TryGetValue(fieldZoneId, out result))
                {
                    Log.Warning($"[{TypeName}] Failed to find zone by ID: {fieldZoneId}].");
                    return true;
                }

                if (result != null)
                {
                    // new
                    String fieldZoneName = GetZoneName(fieldZoneId);
                    IList<KeyValuePair<String, TextReplacement>> filedTags;
                    if (_fieldTags.TryGetValue(fieldZoneName, out filedTags))
                    {
                        for (Int32 i = 0; i < result.Length; i++)
                            result[i] = result[i].ReplaceAll(filedTags, _customTags);
                    }
                    else
                    {
                        for (Int32 i = 0; i < result.Length; i++)
                            result[i] = result[i].ReplaceAll(_customTags);
                    }

                    FF9TextTool.SetFieldText(result);
                    FF9TextTool.ClearTableText();
                }

                return true;
            }
            catch (Exception ex)
            {
                _initialized = true;
                Log.Error(ex, $"[{TypeName}] Failed to import resource.");
                return false;
            }
        }
    }
}
