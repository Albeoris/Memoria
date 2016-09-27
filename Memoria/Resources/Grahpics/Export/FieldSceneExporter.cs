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
                {
                    ExportMapSafe(map);
                    return;
                }

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
                if (Directory.Exists(outputDirectory))
                {
                    Log.Warning($"[FieldSceneExporter] Export was skipped bacause a directory already exists: [{outputDirectory}].");
                    return;
                }

                Log.Message("[FieldSceneExporter] Exporting [{0}]...", mapName);

                BGSCENE_DEF scene = new BGSCENE_DEF(true);
                scene.LoadEBG(null, relativePath, mapName);

                //String directoryPath = Path.GetDirectoryName(outputPath);
                Directory.CreateDirectory(outputDirectory);

                Int32 textureWidth = (Int32)(scene.overlayList.SelectMany(s=>s.spriteList).Max(s => s.offY) + scene.SPRITE_H);
                Int32 textureHeight = (Int32)(scene.overlayList.SelectMany(s => s.spriteList).Max(s => s.offX) + scene.SPRITE_W);
                for (Int32 index = 0; index < scene.overlayList.Count; index++)
                {
                    BGOVERLAY_DEF overlay = scene.overlayList[index];
                    String outputPath = outputDirectory + $"Overlay{index}.png";
                    ExportOverlay(overlay, scene.atlas, outputPath, textureWidth, textureHeight, scene);
                    return;
                }

                //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(scene.atlas), outputPath);

                Log.Message("[FieldSceneExporter] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FieldSceneExporter] Failed to export map [{0}].", mapName);
            }
        }

        private static void ExportOverlay(BGOVERLAY_DEF overlay, Texture2D atlas, String outputPath, Int32 textureWidth, Int32 textureHeight, BGSCENE_DEF scene)
        {
            RenderTexture oldTarget = Camera.main.targetTexture;
            RenderTexture oldActive = RenderTexture.active;

            textureWidth = (Int32)(overlay.spriteList.Max(s => s.offX) + scene.SPRITE_W);
            textureHeight =(Int32)( overlay.spriteList.Max(s => s.offY) + scene.SPRITE_H);

            Texture2D result = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
            RenderTexture rt = RenderTexture.GetTemporary(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32);
            try
            {
                Camera.main.targetTexture = rt;
                Graphics.Blit(atlas, rt);

                //Log.Message("--------------------------------");
                Log.Message(overlay.transform?.name);
                //Log.Message("alpha;atlasX;atlasY;clutX;clutY;depth;h;w;u;v;offX;offY;oriData;pad;res;startOffset;texX;texY;trans;localPosition;name");
                RenderTexture.active = rt;
                foreach (BGSPRITE_LOC_DEF s in overlay.spriteList)
                {
                    //Log.Message($"{s.alpha};{s.atlasX};{s.atlasY};{s.clutX};{s.clutY};{s.depth};{s.h};{s.w};{s.u};{s.v};{s.offX};{s.offY};{s.oriData};{s.pad};{s.res};{s.startOffset};{s.texX};{s.texY};{s.trans};{s.transform?.localPosition};{s.transform?.name}");
                    //uint x, y, x2, y2;
                    //if (scene.SPRITE_W > 16)
                    //{
                    //    x = s.atlasX;
                    //    y = scene.ATLAS_H - s.atlasY;
                    //    x2 = s.atlasX + scene.SPRITE_W;
                    //    y2 = scene.ATLAS_H - (s.atlasY + scene.SPRITE_H);
                    //}
                    //else
                    //{
                    //    x = s.atlasX;
                    //    y = s.atlasY;
                    //    x2 = s.atlasX + scene.SPRITE_W;
                    //    y2 = s.atlasY + scene.SPRITE_H;
                    //}
                    //
                    //x = Math.Min(x, x2);
                    //y = Math.Min(y, y2);

                    result.ReadPixels(new Rect(s.atlasX, s.atlasY, scene.SPRITE_W, scene.SPRITE_H), s.offX, s.offY);
                }
            }
            finally
            {
                RenderTexture.active = oldActive;
                Camera.main.targetTexture = oldTarget;
                RenderTexture.ReleaseTemporary(rt);
            }
            TextureHelper.WriteTextureToFile(result, outputPath);
        }

        private static IEnumerable<String> CreateMapList()
        {
            String[] strArray1 = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/FieldMap/mapList.txt", false).text.Split('\n');
            foreach (String str in strArray1)
            {
                if (str == String.Empty)
                    break;

                String[] strArray2 = str.Split(',');
                yield return strArray2[1];
            }
        }
    }
}