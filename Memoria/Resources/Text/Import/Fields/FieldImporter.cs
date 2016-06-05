using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Memoria
{
    public sealed class FieldImporter : TextImporter
    {
        private const String EndingTag = "[ENDN]";
        private const Int32 EndingLength = 6;

        private static String TypeName => nameof(FieldImporter);
        private readonly Dictionary<Int32, String[]> _cache = new Dictionary<Int32, String[]>();
        private readonly ImportFieldTags _formatter = new ImportFieldTags();

        private volatile Boolean _initialized;
        private Int32 _fieldZoneId;
        private String _fieldFileName;
        private String[] _original;
        private TxtEntry[] _external;
        private Dictionary<String, LinkedList<UInt64>> _dic;
        private IList<KeyValuePair<String, TextReplacement>> _customTags;
        private IList<KeyValuePair<String, TextReplacement>> _fieldReplacements;

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
                Log.Warning($"[{TypeName}] Import was skipped bacause a directory does not exist: [{directory}].");
                return;
            }

            Log.Message($"[{TypeName}] Initialization...");

            try
            {
                _dic = new Dictionary<String, LinkedList<UInt64>>();
                _customTags = LoadCustomTags();
                Dictionary<String, IList<KeyValuePair<String, TextReplacement>>> fieldTags = LoadFieldTags();

                foreach (KeyValuePair<Int32, String> pair in FF9DBAll.EventDB)
                {
                    _fieldZoneId = pair.Key;
                    _fieldFileName = _fieldZoneId.ToString("D4", CultureInfo.InvariantCulture) + '_' + pair.Value;

                    if (!ReadEmbadedText(_fieldZoneId, out _original))
                    {
                        _cache[_fieldZoneId] = null;
                        continue;
                    }

                    if (!ReadExternalText(out _external))
                    {
                        Log.Warning($"[{TypeName}] External file not found: [{Path.Combine(ModTextResources.Import.FieldsDirectory, _fieldFileName + ".strings")}]");
                        _cache[_fieldZoneId] = _original;
                        continue;
                    }

                    if (_original.Length != _external.Length)
                    {
                        Log.Warning($"[{TypeName}] Number of lines in files does not match. [Original: {_original.Length}, External: {_external.Length}]");
                        _cache[_fieldZoneId] = _original;
                        continue;
                    }

                    fieldTags.TryGetValue(_fieldFileName, out _fieldReplacements);
                    _cache[_fieldZoneId] = MergeEntries();
                }

                ResolveReferences();

                _initialized = true;
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
                _customTags = null;
                _fieldReplacements = null;
            }
        }

        private static IList<KeyValuePair<String, TextReplacement>> LoadCustomTags()
        {
            String inputPath = ModTextResources.Import.FieldTags;
            return ReadTagReplacements(inputPath);
        }

        private static Dictionary<String, IList<KeyValuePair<String, TextReplacement>>> LoadFieldTags()
        {
            String fieldsDirectory = ModTextResources.Import.FieldsDirectory;
            if (!Directory.Exists(fieldsDirectory))
                return new Dictionary<String, IList<KeyValuePair<String, TextReplacement>>>(0);

            String[] filePathes = Directory.GetFiles(fieldsDirectory, "*_Tags.strings");
            Dictionary<String, IList<KeyValuePair<String, TextReplacement>>> fieldNames = new Dictionary<String, IList<KeyValuePair<String, TextReplacement>>>(filePathes.Length);
            foreach (String path in filePathes)
            {
                String fileName = Path.GetFileName(path);
                if (fileName == null)
                {
                    Log.Warning($"[{TypeName}] Invalid file path: [{path}].");
                    continue;
                }

                fileName = fileName.Substring(0, fileName.Length - "_Tags.strings".Length);
                fieldNames[fileName] = ReadTagReplacements(path);
            }

            return fieldNames;
        }

        private static IList<KeyValuePair<String, TextReplacement>> ReadTagReplacements(String inputPath)
        {
            if (!File.Exists(inputPath))
                return new List<KeyValuePair<String, TextReplacement>>();

            TxtEntry[] generalNames = TxtReader.ReadStrings(inputPath);
            if (generalNames.IsNullOrEmpty())
                return new List<KeyValuePair<String, TextReplacement>>();

            TextReplacements result = new TextReplacements(generalNames.Length);
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
                } while (target.StartsWith("{$"));

                foreach (UInt64 value in pair.Value)
                {
                    zoneId = (Int32)(value >> 32);
                    lineId = (Int32)(value & UInt32.MaxValue);

                    _cache[zoneId][lineId] = target;
                }
            }
        }

        private String[] MergeEntries()
        {
            String[] result = new String[_original.Length];
            for (int index = 0; index < result.Length; index++)
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

                external = external.ReplaceAll(_fieldReplacements, _customTags, _formatter.SimpleTags.Backward, _formatter.ComplexTags).TrimEnd();
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
            for (int i = str.Length - 1; i >= 0; i--)
            {
                Char ch = str[i];
                if (ch == '\n' || ch == ' ')
                {
                    sb.Insert(0, ch);
                    continue;
                }

                int offset = i - EndingLength + 1;
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
            String inputPath = Path.Combine(ModTextResources.Import.FieldsDirectory, _fieldFileName + ".strings");
            if (!File.Exists(inputPath))
            {
                entries = null;
                return false;
            }

            entries = TxtReader.ReadStrings(inputPath);
            return !entries.IsNullOrEmpty();
        }

        protected override bool LoadInternal()
        {
            Int32 fieldZoneId = FF9TextToolAccessor.GetFieldZoneId();

            String[] text;
            if (ReadEmbadedText(fieldZoneId, out text))
            {
                FF9TextToolAccessor.SetFieldText(text);
                FF9TextToolAccessor.SetTableText(FF9TextToolInterceptor.ExtractTableText(text));
            }

            return true;
        }

        private static bool ReadEmbadedText(int fieldZoneId, out String[] text)
        {
            String path = EmbadedTextResources.GetCurrentPath("/Field/" + FF9TextToolInterceptor.GetFieldTextFileName(fieldZoneId) + ".mes");
            String raw = EmbadedSentenseLoader.LoadText(path);

            if (raw != null)
            {
                raw = TextOpCodeModifier.Modify(raw);
                text = FF9TextToolInterceptor.ExtractSentense(raw);
                return true;
            }

            text = null;
            return false;
        }

        protected override bool LoadExternal()
        {
            try
            {
                if (!_initialized)
                    return false;

                Int32 fieldZoneId = FF9TextToolAccessor.GetFieldZoneId();

                String[] result;
                if (!_cache.TryGetValue(fieldZoneId, out result))
                {
                    Log.Warning($"[{TypeName}] Failed to find zone by ID: {fieldZoneId}].");
                    return true;
                }

                if (result != null)
                {
                    FF9TextToolAccessor.SetFieldText(result);
                    FF9TextToolAccessor.SetTableText(FF9TextToolInterceptor.ExtractTableText(result));
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