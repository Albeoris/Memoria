using System;

namespace Memoria
{
    public sealed class ExportedTypeAttribute : Attribute
    {
        public readonly String Checksum;

        public ExportedTypeAttribute(String checksum)
        {
            Checksum = checksum;
        }
    }
}