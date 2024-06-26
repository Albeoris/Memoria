using Memoria.Prime;
using Memoria.Prime.Exceptions;
using System;
using System.IO;

namespace Memoria.Assets
{
    public readonly struct TextResourceReference
    {
        public String Value { get; }

        public TextResourceReference(String filePath)
        {
            Exceptions.ThrowIfNullOrEmprty(filePath, nameof(filePath));
            Value = filePath;
        }

        public Boolean Equals(TextResourceReference other) => Value == other.Value;
        public override Boolean Equals(object obj) => obj is TextResourceReference other && Equals(other);
        public override Int32 GetHashCode() => (Value != null ? Value.GetHashCode() : 0);
        public override String ToString() => Value;

        public bool IsExists(out TextResourcePath existingFile)
        {
            foreach (TextResourceFormat format in EnumCache<TextResourceFormat>.Values)
            {
                TextResourcePath path = new(this, format);
                if (File.Exists(path.Value))
                {
                    existingFile = path;
                    return true;
                }
            }

            existingFile = default;
            return false;
        }
    }
}
