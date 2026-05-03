using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Memoria.Assets
{
    public static class GraphicResourceExporter
    {
        public static void ExportSafe()
        {
            try
            {
                if (!Configuration.Export.Graphics)
                {
                    Log.Message("[GraphicResourceExporter] Pass through {Configuration.Export.Graphics = 0}.");
                    return;
                }

                foreach (String name in GraphicResources.AtlasList.Keys)
                {
                    String path = GraphicResources.Embedded.GetAtlasPath(name);
                    UIAtlas atlas = AssetManager.Load<UIAtlas>(path, true);
                    if (atlas != null)
                    {
                        path = GraphicResources.Export.GetAtlasPath(name);
                        ExportAtlasSafe(path, atlas);
                    }
                    else
                    {
                        Sprite[] spriteList = Resources.LoadAll<Sprite>(path);
                        if (spriteList == null || spriteList.Length == 0)
                        {
                            Log.Message($"[GraphicResourceExporter] Failed to export '{path}' as an atlas or a sprite list");
                            continue;
                        }
                        path = GraphicResources.Export.GetAtlasPath(name);
                        ExportSpriteListSafe(path, spriteList);
                    }
                }

                CommonSPSSystem.ExportAllSPSTextures(Path.Combine(Configuration.Export.Path, "EffectSPS"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GraphicResourceExporter] Failed to export graphic resources.");
            }
        }

        public static void ExportAtlasSafe(String outputDirectory, UIAtlas atlas)
        {
            try
            {
                String outputPath = outputDirectory + ".png";
                if (File.Exists(outputPath))
                {
                    Log.Warning("[GraphicResourceExporter] Export was skipped because a file already exists: [{0}].", outputPath);
                    return;
                }

                Directory.CreateDirectory(outputDirectory);

                Texture2D texture = TextureHelper.CopyAsReadable(atlas.texture);
                TextureHelper.WriteTextureToFile(texture, outputPath);
                foreach (UISpriteData sprite in atlas.spriteList)
                {
                    Texture2D fragment = TextureHelper.GetFragment(texture, sprite.x, texture.height - sprite.y - sprite.height, sprite.width, sprite.height);
                    TextureHelper.WriteTextureToFile(fragment, Path.Combine(outputDirectory, sprite.name + ".png"));
                }

                String outputPathTPSheet = outputDirectory + ".tpsheet";
                String tpsheetText = ":format=40000\n"
                //  + ":texture=" + texture.name + "\n"
                //  + ":normalmap=\n";
                    + ":size=" + texture.width + "x" + texture.height + "\n";
                foreach (UISpriteData sprite in atlas.spriteList)
                {
                    tpsheetText += sprite.name + ";" + sprite.x + ";" + sprite.y + ";" + sprite.width + ";" + sprite.height;
                    tpsheetText += ";0;0"; // pivotX & pivotY, unused
                    tpsheetText += ";" + sprite.paddingLeft + ";" + sprite.paddingRight + ";" + sprite.paddingTop + ";" + sprite.paddingBottom;
                    tpsheetText += ";" + sprite.borderBottom + ";" + sprite.borderRight + ";" + sprite.borderTop + ";" + sprite.borderBottom;
                    tpsheetText += "\n";
                }
                File.WriteAllText(outputPathTPSheet, tpsheetText);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GraphicResourceExporter] Failed to export atlas [{0}].", atlas.name);
            }
        }

        public static void ExportSpriteListSafe(String outputDirectory, Sprite[] spriteList)
        {
            try
            {
                String outputPath = outputDirectory + ".png";
                if (File.Exists(outputPath))
                {
                    Log.Warning("[GraphicResourceExporter] Export was skipped because a file already exists: [{0}].", outputPath);
                    return;
                }

                Directory.CreateDirectory(outputDirectory);

                Texture2D sharedTexture = TextureHelper.CopyAsReadable(spriteList[0].texture);
                TextureHelper.WriteTextureToFile(sharedTexture, outputDirectory + ".png");
                foreach (Sprite sprite in spriteList)
                {
                    Texture2D texture = TextureHelper.GetFragment(sharedTexture, (Int32)sprite.rect.x, (Int32)sprite.rect.y, (Int32)sprite.rect.width, (Int32)sprite.rect.height);
                    TextureHelper.WriteTextureToFile(texture, Path.ChangeExtension(Path.Combine(outputDirectory, sprite.name), ".png"));
                }

                String outputPathTPSheet = outputDirectory + ".tpsheet";
                String tpsheetText = ":format=40000\n"
                //  + ":texture=" + texture.name + "\n"
                //  + ":normalmap=\n";
                    + ":size=" + sharedTexture.width + "x" + sharedTexture.height + "\n";
                foreach (Sprite sprite in spriteList)
                {
                    tpsheetText += sprite.name + ";" + (Int32)sprite.rect.x + ";" + (Int32)sprite.rect.y + ";" + (Int32)sprite.rect.width + ";" + (Int32)sprite.rect.height;
                    tpsheetText += ";0;0"; // pivotX & pivotY, unused
                    tpsheetText += ";0;0;0;0"; // paddings, nulled for sprite lists
                    tpsheetText += ";0;0;0;0"; // borders, nulled for sprite lists
                    tpsheetText += "\n";
                }
                File.WriteAllText(outputPathTPSheet, tpsheetText);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GraphicResourceExporter] Failed to export atlas [{0}].", Path.GetFileName(outputDirectory));
            }
        }

        public static void ExportPartialAtlasSafe(String outputPath, UIAtlas atlas)
        {
            try
            {
                if (atlas.spriteList.Count == 0)
                    return;
                if (File.Exists(outputPath))
                {
                    Log.Warning("[GraphicResourceExporter] Export was skipped because a file already exists: [{0}].", outputPath);
                    return;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                Dictionary<Texture, Texture2D> readableTextures = new Dictionary<Texture, Texture2D>();
                List<Vector2> newPos = new List<Vector2>();
                Int32 x = 0;
                Int32 y = 0;
                Int32 w = 0;
                Int32 h = 0;
                foreach (UISpriteData sprite in atlas.spriteList)
                    w = Math.Max(w, sprite.width);
                for (Int32 i = 0; i < atlas.spriteList.Count; i++)
                {
                    UISpriteData sprite = atlas.spriteList[i];
                    if (x + sprite.width > w)
                    {
                        x = 0;
                        y = h;
                    }
                    newPos.Add(new Vector2(x, y));
                    x += sprite.width;
                    h = Math.Max(h, y + sprite.height);
                }
                Texture2D genTexture = new Texture2D(w, h);
                for (Int32 i = 0; i < atlas.spriteList.Count; i++)
                {
                    UISpriteData sprite = atlas.spriteList[i];
                    Texture baseTexture = sprite.texture != null ? sprite.texture : atlas.texture;
                    if (!readableTextures.TryGetValue(baseTexture, out Texture2D rTexture))
                    {
                        rTexture = TextureHelper.CopyAsReadable(baseTexture);
                        readableTextures[baseTexture] = rTexture;
                    }
                    Color[] colors = rTexture.GetPixels(sprite.x, rTexture.height - sprite.y - sprite.height, sprite.width, sprite.height);
                    genTexture.SetPixels((Int32)newPos[i].x, genTexture.height - (Int32)newPos[i].y - sprite.height, sprite.width, sprite.height, colors);
                }
                genTexture.Apply();
                TextureHelper.WriteTextureToFile(genTexture, outputPath);
                UnityEngine.Object.DestroyImmediate(genTexture);

                String outputPathTPSheet = Path.ChangeExtension(outputPath, ".tpsheet");
                String tpsheetText = ":format=40000\n"
                //  + ":texture=" + texture.name + "\n"
                //  + ":normalmap=\n";
                    + ":size=" + w + "x" + h + "\n"
                    + ":append=true\n";
                for (Int32 i = 0; i < atlas.spriteList.Count; i++)
                {
                    UISpriteData sprite = atlas.spriteList[i];
                    tpsheetText += sprite.name + ";" + (Int32)newPos[i].x + ";" + (Int32)newPos[i].y + ";" + sprite.width + ";" + sprite.height;
                    tpsheetText += ";0;0"; // pivotX & pivotY, unused
                    tpsheetText += ";" + sprite.paddingLeft + ";" + sprite.paddingRight + ";" + sprite.paddingTop + ";" + sprite.paddingBottom;
                    tpsheetText += ";" + sprite.borderBottom + ";" + sprite.borderRight + ";" + sprite.borderTop + ";" + sprite.borderBottom;
                    tpsheetText += "\n";
                }
                File.WriteAllText(outputPathTPSheet, tpsheetText);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GraphicResourceExporter] Failed to export partial atlas [{0}].", atlas.name);
            }
        }

        public static void ExportPartialSpriteListSafe(String outputPath, List<Sprite> spriteList)
        {
            try
            {
                if (spriteList.Count == 0)
                    return;
                if (File.Exists(outputPath))
                {
                    Log.Warning("[GraphicResourceExporter] Export was skipped because a file already exists: [{0}].", outputPath);
                    return;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                Texture2D sharedTexture = TextureHelper.CopyAsReadable(spriteList[0].texture, true);
                List<Vector2> newPos = new List<Vector2>();
                Int32 x = 0;
                Int32 y = 0;
                Int32 w = 0;
                Int32 h = 0;
                foreach (Sprite sprite in spriteList)
                    w = Math.Max(w, (Int32)sprite.rect.width);
                for (Int32 i = 0; i < spriteList.Count; i++)
                {
                    Sprite sprite = spriteList[i];
                    if (x + (Int32)sprite.rect.width > w)
                    {
                        x = 0;
                        y = h;
                    }
                    newPos.Add(new Vector2(x, y));
                    x += (Int32)sprite.rect.width;
                    h = Math.Max(h, y + (Int32)sprite.rect.height);
                }
                Texture2D genTexture = new Texture2D(w, h);
                for (Int32 i = 0; i < spriteList.Count; i++)
                {
                    Sprite sprite = spriteList[i];
                    Color[] colors = sharedTexture.GetPixels((Int32)sprite.rect.x, (Int32)sprite.rect.y, (Int32)sprite.rect.width, (Int32)sprite.rect.height);
                    genTexture.SetPixels((Int32)newPos[i].x, (Int32)newPos[i].y, (Int32)sprite.rect.width, (Int32)sprite.rect.height, colors);
                }
                genTexture.Apply();
                TextureHelper.WriteTextureToFile(genTexture, outputPath);
                UnityEngine.Object.DestroyImmediate(genTexture);

                String outputPathTPSheet = Path.ChangeExtension(outputPath, ".tpsheet");
                String tpsheetText = ":format=40000\n"
                //  + ":texture=" + texture.name + "\n"
                //  + ":normalmap=\n";
                    + ":size=" + w + "x" + h + "\n"
                    + ":append=true\n";
                for (Int32 i = 0; i < spriteList.Count; i++)
                {
                    Sprite sprite = spriteList[i];
                    tpsheetText += sprite.name + ";" + (Int32)newPos[i].x + ";" + (Int32)newPos[i].y + ";" + (Int32)sprite.rect.width + ";" + (Int32)sprite.rect.height;
                    tpsheetText += ";0;0"; // pivotX & pivotY, unused
                    tpsheetText += ";0;0;0;0"; // paddings, nulled for sprite lists
                    tpsheetText += ";0;0;0;0"; // borders, nulled for sprite lists
                    tpsheetText += "\n";
                }
                File.WriteAllText(outputPathTPSheet, tpsheetText);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GraphicResourceExporter] Failed to export partial atlas [{0}].", Path.GetFileName(outputPath));
            }
        }
    }
}
