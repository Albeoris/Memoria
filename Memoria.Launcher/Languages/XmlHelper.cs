using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Memoria.Launcher
{
    public static class XmlHelper
    {
        public static XmlElement CreateDocument(string rootName)
        {
            XmlDocument doc = new XmlDocument();

            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(dec);

            XmlElement root = doc.CreateElement(rootName);
            doc.AppendChild(root);

            return root;
        }

        public static XmlElement LoadDocument(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            return doc.GetDocumentElement();
        }

        public static XmlElement TryLoadDocument(string xmlPath)
        {
            if (!File.Exists(xmlPath))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            return doc.GetDocumentElement();
        }

        public static XmlElement LoadEmbadedDocument(Assembly assembly, String name)
        {
            using (Stream input = assembly.GetManifestResourceStream(name))
            {
                if (input == null)
                    return null;

                XmlDocument doc = new XmlDocument();
                doc.Load(input);
                return doc.GetDocumentElement();
            }
        }
    }
}
