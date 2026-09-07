#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModFixChangelogStore
    {
        internal const String FileName = "Memoria.Fixer.Changelog.json";
        private readonly String _path;
        private readonly List<ModFixChangelogEntry> _entries;

        public ModFixChangelogStore(String modDirectory)
        {
            _path = Path.Combine(modDirectory ?? throw new ArgumentNullException(nameof(modDirectory)), FileName);
            _entries = Load(_path);
        }

        public ModFixChangelogEntry FindCurrent(ModValidationFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (!File.Exists(file.FullPath))
                return null;

            List<ModFixChangelogEntry> candidates = _entries.Where(entry => String.Equals(entry.RelativePath, file.RelativePath, StringComparison.OrdinalIgnoreCase)).ToList();
            if (candidates.Count == 0)
                return null;
            String hash = ComputeSha256(file.FullPath);
            return candidates.LastOrDefault(entry => String.Equals(entry.FileSha256, hash, StringComparison.OrdinalIgnoreCase));
        }

        public ModFixChangelogEntry Append(ModFileFixResult fixResult)
        {
            if (fixResult == null)
                throw new ArgumentNullException(nameof(fixResult));
            if (!fixResult.Changed)
                throw new ArgumentException("A changelog entry requires at least one change.", nameof(fixResult));

            ModFixChangelogEntry entry = new()
            {
                Timestamp = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
                RelativePath = fixResult.File.RelativePath,
                FileSha256 = ComputeSha256(fixResult.File.FullPath),
                FixerName = fixResult.FixerName,
                Changes = fixResult.Changes.ToList()
            };
            _entries.Add(entry);
            Save();
            return entry;
        }

        private void Save()
        {
            ChangelogDocument document = new() { Entries = _entries };
            String temporaryPath = _path + ".tmp";
            try
            {
                using (FileStream stream = new(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    CreateSerializer().WriteObject(stream, document);
                if (File.Exists(_path))
                    File.Replace(temporaryPath, _path, null);
                else
                    File.Move(temporaryPath, _path);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }

        private static List<ModFixChangelogEntry> Load(String path)
        {
            if (!File.Exists(path))
                return new List<ModFixChangelogEntry>();
            using (FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ChangelogDocument document = (ChangelogDocument)CreateSerializer().ReadObject(stream);
                return document?.Entries ?? new List<ModFixChangelogEntry>();
            }
        }

        private static DataContractJsonSerializer CreateSerializer() => new(typeof(ChangelogDocument));

        private static String ComputeSha256(String path)
        {
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                return BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", String.Empty);
        }

        [DataContract]
        private sealed class ChangelogDocument
        {
            [DataMember(Order = 1)] public List<ModFixChangelogEntry> Entries { get; set; } = new();
        }
    }
}
