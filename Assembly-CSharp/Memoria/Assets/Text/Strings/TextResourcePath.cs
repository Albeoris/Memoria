using Memoria.Prime.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria.Assets
{
    public readonly struct TextResourcePath
    {
        public String Value { get; }
        public TextResourceFormat Format { get; }

        public TextResourcePath(TextResourceReference reference, TextResourceFormat format)
        {
            Exceptions.ThrowIfNullOrEmprty(reference.Value, nameof(reference));
            Exceptions.ThrowIfDefault(format, nameof(format));

            Value = Path.ChangeExtension(reference.Value, format.GetFileExtension());
            Format = format;
        }

        public static TextResourcePath ForExport(String outputPath)
        {
            TextResourceReference reference = new(outputPath);
            return new TextResourcePath(reference, Configuration.Export.TextFileFormat);
        }
        
        public static TextResourcePath ForImportExistingFile(String existingPath)
        {
            if (!File.Exists(existingPath))
                throw new FileNotFoundException(existingPath);

            String extension = Path.GetExtension(existingPath);
            TextResourceFormat format = TextResourceFormatHelper.ResolveFileFormat(extension);
            TextResourceReference reference = new(existingPath);
            return new TextResourcePath(reference, format);
        }

        public bool Equals(TextResourcePath other) => Value == other.Value;
        public override Boolean Equals(object obj) => obj is TextResourcePath other && Equals(other);
        public override Int32 GetHashCode() => (Value != null ? Value.GetHashCode() : 0);
        public override String ToString() => Value;

        public ITextFormatter GetFormatter()
        {
            return Format switch
            {
                TextResourceFormat.Strings => TextStringsFormatter.Instance,
                TextResourceFormat.Resjson => TextResjsonFormatter.Instance,
                _ => throw new NotSupportedException(Format.ToString())
            };
        }

        public void WriteAll(IList<TxtEntry> entries)
        {
            for (Int32 i = 0; i < entries.Count; i++)
                entries[i].TryUpdateIndex(i);
            
            ITextFormatter formatter = GetFormatter();
            formatter.GetWriter().WriteAll(Value, entries);
        }
        
        public TxtEntry[] ReadAll()
        {
            ITextFormatter formatter = GetFormatter();
            return formatter.GetReader().ReadAll(Value);
        }
    }
}