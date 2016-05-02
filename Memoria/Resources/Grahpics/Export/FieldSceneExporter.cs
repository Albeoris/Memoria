using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Memoria
{
    public static class FieldSceneExporter
    {
        public static void ExportSafe()
        {
            try
            {
                if (!Configuration.Export.Field)
                {
                    Log.Message("[FieldSceneExporter] Pass through {Configuration.Export.Field = 0}.");
                    return;
                }

                Boolean old = AssetManager.UseBundles;
                AssetManager.UseBundles = true;

                foreach (String map in CreateMapList())
                    ExportMapSafe(map);

                AssetManager.UseBundles = old;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FieldSceneExporter] Failed to export field resources.");
            }
        }

        private static void ExportMapSafe(String mapName)
        {
            try
            {
                String relativePath = FieldMap.GetMapResourcePath(mapName);
                String outputDirectory = Path.Combine(Configuration.Export.Path, relativePath);
                String outputPath = outputDirectory + "Atlas.png";
                if (File.Exists(outputPath))
                {
                    Log.Warning($"[FieldSceneExporter] Export was skipped bacause a file already exists: [{outputPath}].");
                    return;
                }

                Log.Message("[FieldSceneExporter] Exporting [{0}]...", mapName);

                BGSCENE_DEF scene = new BGSCENE_DEF(true);
                scene.LoadEBG(null, relativePath, mapName);
                TextureResources.WriteTextureToFile(scene.atlas, outputPath);

                Log.Message("[FieldSceneExporter] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FieldSceneExporter] Failed to export map [{0}].", mapName);
            }
        }

        private static IEnumerable<String> CreateMapList()
        {
            string[] strArray1 = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/FieldMap/mapList.txt", false).text.Split('\n');
            foreach (string str in strArray1)
            {
                if (str == string.Empty)
                    break;

                string[] strArray2 = str.Split(',');
                yield return strArray2[1];
            }
        }
    }
}