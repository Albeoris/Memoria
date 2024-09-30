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
                        ExportAtlasSafe(path, atlas, true);
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
                        ExportSpriteListSafe(path, spriteList, true);
                    }
                }

                CommonSPSSystem.ExportAllSPSTextures(Path.Combine(Configuration.Export.Path, "EffectSPS"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GraphicResourceExporter] Failed to export graphic resources.");
            }
        }

        public static void ExportAtlasSafe(String outputDirectory, UIAtlas atlas, Boolean exportFragments)
        {
            try
            {
                String outputPath = outputDirectory + ".png";
                if (File.Exists(outputPath))
                {
                    Log.Warning("[GraphicResourceExporter] Export was skipped because a file already exists: [{0}].", outputPath);
                    return;
                }

                if (exportFragments)
                    Directory.CreateDirectory(outputDirectory);
                else
                    Directory.CreateDirectory(Path.GetDirectoryName(outputDirectory));

                Texture2D texture = TextureHelper.CopyAsReadable(atlas.texture);
                TextureHelper.WriteTextureToFile(texture, outputPath);
                if (exportFragments)
                {
                    foreach (UISpriteData sprite in atlas.spriteList)
                    {
                        Texture2D fragment = TextureHelper.GetFragment(texture, sprite.x, texture.height - sprite.y - sprite.height, sprite.width, sprite.height);
                        TextureHelper.WriteTextureToFile(fragment, Path.Combine(outputDirectory, sprite.name + ".png"));
                    }
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

        public static void ExportSpriteListSafe(String outputDirectory, Sprite[] spriteList, Boolean exportFragments)
        {
            try
            {
                String outputPath = outputDirectory + ".png";
                if (File.Exists(outputPath))
                {
                    Log.Warning("[GraphicResourceExporter] Export was skipped because a file already exists: [{0}].", outputPath);
                    return;
                }

                if (exportFragments)
                    Directory.CreateDirectory(outputDirectory);
                else
                    Directory.CreateDirectory(Path.GetDirectoryName(outputDirectory));

                Texture2D sharedTexture = TextureHelper.CopyAsReadable(spriteList[0].texture);
                TextureHelper.WriteTextureToFile(sharedTexture, outputDirectory + ".png");
                if (exportFragments)
                {
                    foreach (Sprite sprite in spriteList)
                    {
                        Texture2D texture = TextureHelper.GetFragment(sharedTexture, (Int32)sprite.rect.x, (Int32)sprite.rect.y, (Int32)sprite.rect.width, (Int32)sprite.rect.height);
                        TextureHelper.WriteTextureToFile(texture, Path.ChangeExtension(Path.Combine(outputDirectory, sprite.name), ".png"));
                    }
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
    }
}
