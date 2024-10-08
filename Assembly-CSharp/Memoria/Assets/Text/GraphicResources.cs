using System;
using System.Collections.Generic;
using System.Text;

namespace Memoria.Assets
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

        public static readonly Dictionary<String, String> AtlasList = new Dictionary<String, String>
        {
            // May not be complete and/or correct
            // resources.assets
            { "EndGame Atlas",              "EmbeddedAsset/EndGame/Prefabs/EndGame Atlas" },
            { "QuadMist Text ES Atlas",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_ES" }, // Custom atlas not currently supported: see EndGameResourceManager
            { "QuadMist Text FR Atlas",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_FR" },
            { "QuadMist Text GR Atlas",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_GR" },
            { "QuadMist Text JP Atlas",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_JP" },
            { "QuadMist Text UK Atlas",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_UK" },
            { "Ending_Text_FR_IT_ES_Atlas", "EmbeddedAsset/UI/Atlas/Ending_Text_FR_IT_ES_Atlas" },
            // sharedassets2.assets
            { "Card_Image",                 "EmbeddedAsset/EndGame/Card_Image" }, // Custom atlas not currently supported: see EndGameMain
            { "QuadMist Image Atlas 0",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Image0" }, // Custom atlas not currently supported: see QuadMistResourceManager and QuadMistCardCursor
            { "QuadMist Image Atlas 1",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Image1" },
            { "QuadMist Text IT Atlas",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_IT" },
            { "QuadMist Text US Atlas",     "EmbeddedAsset/QuadMist/Atlas/QuadMist_Text_US" },
            { "Blue Atlas",                 "EmbeddedAsset/UI/Atlas/Blue Atlas" },
            { "Chocograph Atlas",           "EmbeddedAsset/UI/Atlas/Chocograph Atlas" },
            { "Ending_Text_US_JP_GR_Atlas", "EmbeddedAsset/UI/Atlas/Ending_Text_US_JP_GR_Atlas" },
            { "Face Atlas",                 "EmbeddedAsset/UI/Atlas/Face Atlas" },
            { "General Atlas",              "EmbeddedAsset/UI/Atlas/General Atlas" },
            { "Gray Atlas",                 "EmbeddedAsset/UI/Atlas/Gray Atlas" },
            { "Icon Atlas",                 "EmbeddedAsset/UI/Atlas/Icon Atlas" },
            { "Movie Gallery Atlas",        "EmbeddedAsset/UI/Atlas/Movie Gallery Atlas" },
            { "Screen Button Atlas",        "EmbeddedAsset/UI/Atlas/Screen Button Atlas" },
            { "TutorialUI Atlas",           "EmbeddedAsset/UI/Atlas/TutorialUI Atlas" },
        };

        public static class Embedded
        {
            public static String GetAtlasPath(String atlasName)
            {
                if (AtlasList.ContainsKey(atlasName))
                    return AtlasList[atlasName];
                return "EmbeddedAsset/UI/Atlas/" + atlasName;
            }
        }

        public static class Export
        {
            public static String GetAtlasPath(String atlasName)
            {
                String path = Configuration.Export.Path.Replace('\\', '/');
                if (path.Length > 0 && path[path.Length - 1] != '/')
                    path += "/";
                return $"{path}UI/Atlas/{atlasName}";
            }
        }

        public static class Import
        {
            public static String GetAtlasPath(String atlasName)
            {
                String path = Configuration.Import.Path.Replace('\\', '/');
                if (path.Length > 0 && path[path.Length - 1] != '/')
                    path += "/";
                return $"{path}UI/Atlas/{atlasName}";
            }
        }
    }
}
