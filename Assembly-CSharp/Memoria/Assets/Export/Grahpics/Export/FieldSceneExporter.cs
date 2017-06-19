using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Memoria.Prime;
using Memoria.Prime.PsdFile;
using UnityEngine;

namespace Memoria.Assets
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
                    //return;
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
                    //return;
                }

                Log.Message("[FieldSceneExporter] Exporting [{0}]...", mapName);

                BGSCENE_DEF scene = new BGSCENE_DEF(true);
                scene.LoadEBG(null, relativePath, mapName);

                //String directoryPath = Path.GetDirectoryName(outputPath);
                Directory.CreateDirectory(outputDirectory);

                Texture2D atlasTexture = TextureHelper.CopyAsReadable(scene.atlas);
                Int32 factor = (Int32)scene.SPRITE_W / 16;
                Int32 textureWidth = (Int32)(scene.overlayList.SelectMany(s => s.spriteList).Max(s => s.offY * factor) + scene.SPRITE_H);
                Int32 textureHeight = (Int32)(scene.overlayList.SelectMany(s => s.spriteList).Max(s => s.offX * factor) + scene.SPRITE_W);

                using (Stream output = File.Create(outputDirectory + "test.psd"))
                {
                    PsdFile file = new PsdFile();
                    file.BitDepth = 8;
                    file.ChannelCount = 4;
                    file.ColorMode = PsdColorMode.Rgb;
                    file.RowCount = textureWidth;
                    file.ColumnCount = textureHeight;
                    file.Resolution = new ResolutionInfo
                    {
                        Name = "ResolutionInfo ",

                        HDpi = new UFixed1616(72, 0),
                        HResDisplayUnit = ResolutionInfo.ResUnit.PxPerInch,
                        HeightDisplayUnit = ResolutionInfo.Unit.Centimeters,

                        VDpi = new UFixed1616(72, 0),
                        VResDisplayUnit = ResolutionInfo.ResUnit.PxPerInch,
                        WidthDisplayUnit = ResolutionInfo.Unit.Centimeters
                    };

                    file.BaseLayer.Name = "Base";
                    for (Int16 i = 0; i < 4; i++)
                    {
                        Channel channel = new Channel(i, file.BaseLayer);
                        channel.ImageCompression = file.ImageCompression;
                        channel.Length = file.RowCount * Util.BytesPerRow(file.BaseLayer.Rect.Size, file.BitDepth);
                        channel.ImageData = new Byte[channel.Length];
                        file.BaseLayer.Channels.Add(channel);
                    }

                    file.BaseLayer.Channels.Last().ID = -1;

                    for (Int32 index = scene.overlayList.Count - 1; index >= 0; index--) //scene.overlayList.Count
                    {
                        BGOVERLAY_DEF overlay = scene.overlayList[index];
                        String outputPath = outputDirectory + $"Overlay{index}.png";
                        ExportOverlay(overlay, atlasTexture, outputPath, scene, file);
                        //return;
                    }

                    file.Save(output, Encoding.UTF8);
                }

                string strings = $"[PsdSection]\nLayerOrder=name\nReversed = 0";
                System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(outputDirectory, "psd.meta"));
                sw.WriteLine(strings);
                sw.Close();


                TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(scene.atlas), Path.Combine(outputDirectory, "atlas.png"));

                Log.Message("[FieldSceneExporter] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FieldSceneExporter] Failed to export map [{0}].", mapName);
            }
        }

        private static void ExportOverlay(BGOVERLAY_DEF overlay, Texture2D atlas, String outputPath, BGSCENE_DEF scene, PsdFile psd)
        {
            Int32 factor = (Int32)scene.SPRITE_W / 16;
            Log.Message($"Transform: {overlay.transform?.name}, Factor: {factor}");
            Log.Message($"SPRITE_H: {scene.SPRITE_H:D2} SPRITE_W {scene.SPRITE_W:D2}");
            if (overlay.spriteList.Count < 1)
            {
                Log.Message("Overlay is empty.");
                return;
            }

            Layer layer = new Layer(psd);
            psd.Layers.Add(layer);
            Int32 textureWidth = (Int32)(overlay.spriteList.Max(s => s.offX * factor) + scene.SPRITE_W);
            Int32 textureHeight = (Int32)(overlay.spriteList.Max(s => s.offY * factor) + scene.SPRITE_H);
            Texture2D result = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
            //Log.Message("depth;h;w;u;v;offX;offY;oriData;pad;res;startOffset;texX;texY;trans;localPosition;name");

            Channel r = new Channel(0, layer);
            Channel g = new Channel(1, layer);
            Channel b = new Channel(2, layer);
            Channel a = new Channel(-1, layer);
            layer.Channels.Add(r);
            layer.Channels.Add(g);
            layer.Channels.Add(b);
            layer.Channels.Add(a);

            Int32 channelSize = textureWidth * textureHeight;
            r.Length = channelSize;
            g.Length = channelSize;
            b.Length = channelSize;
            a.Length = channelSize;
            r.ImageData = new Byte[channelSize];
            g.ImageData = new Byte[channelSize];
            b.ImageData = new Byte[channelSize];
            a.ImageData = new Byte[channelSize];
            
            Color[] transpPixels = new Color[textureWidth * textureHeight];
            Color transpPixel = new Color(1.0f,1.0f, 1.0f, 0.0f);
            for (var i = 0; i < textureHeight * textureWidth; i++)
            {
                transpPixels[i] = transpPixel;
            }
            result.SetPixels(transpPixels);

            foreach (BGSPRITE_LOC_DEF s in overlay.spriteList)
            {
                //Log.Message($"{s.depth};{s.h};{s.w};{s.u};{s.v};{s.offX};{s.offY};{s.oriData};{s.pad};{s.res};{s.startOffset};{s.texX};{s.texY};{s.trans};{s.transform?.localPosition};{s.transform?.name}");

                Int32 w = (Int32)scene.SPRITE_W;
                Int32 h = (Int32)scene.SPRITE_H;

                Int32 sx = s.atlasX;
                Int32 sy = (Int32)(scene.ATLAS_H - s.atlasY - scene.SPRITE_H);
                Color[] pixels = atlas.GetPixels(sx, sy, w, h);

                Int32 tx = s.offX * factor;
                Int32 ty = textureHeight - s.offY * factor - (Int32)scene.SPRITE_H;

                result.SetPixels(tx, ty, w, h, pixels);

                for (Int32 y = 0; y < h; y++)
                    for (Int32 x = 0; x < w; x++)
                    {
                        Int32 sourceOffset = (h - y - 1) * w + x;
                        Int32 targetOffset = (s.offY * factor + y) * textureWidth + tx + x;

                        Color color = pixels[sourceOffset];
                        r.ImageData[targetOffset] = (Byte)(color.r * 255);
                        g.ImageData[targetOffset] = (Byte)(color.g * 255);
                        b.ImageData[targetOffset] = (Byte)(color.b * 255);
                        a.ImageData[targetOffset] = (Byte)(color.a * 255);
                    }

                //result.ReadPixels(new Rect(s.atlasX, s.atlasY, scene.SPRITE_W, scene.SPRITE_H), s.offX * factor, textureHeight - s.offY * factor);
            }
            TextureHelper.WriteTextureToFile(result, outputPath);

            layer.Name = Path.GetFileNameWithoutExtension(outputPath);
            layer.Opacity = 255;
            layer.Rect = new Rectangle(overlay.curX * factor, overlay.curY * factor, textureWidth, textureHeight);
            layer.Masks = new MaskInfo();
            layer.BlendingRangesData = new BlendingRanges(layer);
            layer.CreateMissingChannels();
        }

        private static IEnumerable<String> CreateMapList()
        {
            String[] strArray1 = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/FieldMap/mapList.txt", false).text.Split('\n');
            for (Int32 i = 0; i < 100; i++)
            //foreach (String str in strArray1)
            {
                String str = strArray1[i];
                if (str == String.Empty)
                    break;

                String[] strArray2 = str.Split(',');
                yield return strArray2[1];
            }
        }
    }
}