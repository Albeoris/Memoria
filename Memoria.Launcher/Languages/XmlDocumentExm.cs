using System;
using System.Xml;

namespace Memoria.Launcher
{
    public static class XmlDocumentExm
    {
        public static XmlElement GetDocumentElement(this XmlDocument self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            XmlElement element = self.DocumentElement;
            if (element == null)
                throw new ArgumentException("XmlElement was not found.", nameof(self));

            return element;
        }

        public static string FindString(this XmlElement self, string name)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name == String.Empty) throw new ArgumentException(nameof(name));

            XmlAttribute arg = self.Attributes[name];
            return arg?.Value;
        }
    }
}