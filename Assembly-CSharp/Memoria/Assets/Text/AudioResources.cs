using System;
using System.Text;

namespace Memoria.Assets
{
    public static class AudioResources
    {
        public static class Embaded
        {
            public static String GetSoundPath(String relativePath)
            {
                return "Sounds/" + relativePath + ".akb";
            }
        }

        public static class Export
        {
            public static String GetSoundPath(String relativePath)
            {
                String path = Configuration.Export.Path;
                StringBuilder sb = new StringBuilder(path.Length + 32);
                sb.Append(path);
                if (sb.Length > 0 && sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\')
                    sb.Append('/');
                sb.Append("Sounds/");
                sb.Append(relativePath);
                return sb.ToString();
            }
        }

        public static class Import
        {
            public static String GetSoundPath(String relativePath)
            {
                String path = Configuration.Import.Path;
                StringBuilder sb = new StringBuilder(path.Length + 32);
                sb.Append(path);
                if (sb.Length > 0 && sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\')
                    sb.Append('/');
                sb.Append("Sounds/");
                sb.Append(relativePath);
                return sb.ToString();
            }
        }
    }
}