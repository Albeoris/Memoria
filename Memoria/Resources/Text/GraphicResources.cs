using System;
using System.Text;

namespace Memoria
{
    public static class GraphicResources
    {
        public const String GrayAtlasName = "Gray Atlas";
        public const String BlueAtlasName = "Blue Atlas";
        public const String IconAtlasName = "Icon Atlas";
        public const String GeneralAtlasName = "General Atlas";
        public const String ScreenButtonAtlasName = "Screen Button Atlas";
        public const String TutorialUIAtlasName = "TutorialUI Atlas";
        public const String EndGameAtlasName = "EndGame Atlas";
        public const String FaceAtlasName = "Face Atlas";
        public const String ChocographAtlasName = "Chocograph Atlas";
        public const String MovieGalleryAtlasName = "Movie Gallery Atlas";
        public const String QuadMistImageAtlas0Name = "QuadMist Image Atlas 0";
        public const String QuadMistImageAtlas1Name = "QuadMist Image Atlas 1";
        public const String QuadMistTextUSAtlasName = "QuadMist Text US Atlas";
        public const String QuadMistTextITAtlasName = "QuadMist Text IT Atlas";
        public const String EndingTextUsJpGrAtlasName = "Ending_Text_US_JP_GR_Atlas";

        public static class Embaded
        {
            public static String GetAtlasPath(String atlasName)
            {
                return "EmbeddedAsset/UI/Atlas/" + atlasName;
            }
        }

        public static class Export
        {
            public static String GetAtlasPath(String atlasName)
            {
                String path = Configuration.Export.Path;
                StringBuilder sb = new StringBuilder(path.Length + 32);
                sb.Append(path);
                if (sb.Length > 0 && sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\')
                    sb.Append('/');
                sb.Append("/UI/Atlas/");
                sb.Append(atlasName);
                return sb.ToString();
            }
        }

        public static class Import
        {
            public static String GetAtlasPath(String atlasName)
            {
                String path = Configuration.Import.Path;
                StringBuilder sb = new StringBuilder(path.Length + 32);
                sb.Append(path);
                if (sb.Length > 0 && sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\')
                    sb.Append('/');
                sb.Append("/UI/Atlas/");
                sb.Append(atlasName);
                return sb.ToString();
            }
        }
    }
}