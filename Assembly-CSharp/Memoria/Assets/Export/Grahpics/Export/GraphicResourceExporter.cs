using System;
using System.Collections.Generic;
using System.IO;
using Memoria.Prime;
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

                foreach (String name in EnumerateAtlases())
                {
                    String path = GraphicResources.Embedded.GetAtlasPath(name);
                    UIAtlas atlas = Resources.Load(path, typeof(UIAtlas)) as UIAtlas;
                    if (atlas != null)
                    {
                        path = GraphicResources.Export.GetAtlasPath(name);
                        ExportAtlasSafe(path, atlas);
                    }
                }
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
                if (Directory.Exists(outputDirectory))
                {
                    Log.Warning("[GraphicResourceExporter] Export was skipped bacause a directory already exists: [{0}].", outputDirectory);
                    return;
                }

                String outputPath = outputDirectory + ".png";
                if (File.Exists(outputPath))
                {
                    Log.Warning("[GraphicResourceExporter] Export was skipped bacause a file already exists: [{0}].", outputPath);
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
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GraphicResourceExporter] Failed to export atlas [{0}].", atlas.name);
            }
        }

        private static IEnumerable<String> EnumerateAtlases()
        {
            yield return GraphicResources.GrayAtlasName;
            yield return GraphicResources.BlueAtlasName;
            yield return GraphicResources.IconAtlasName;
            yield return GraphicResources.GeneralAtlasName;
            yield return GraphicResources.ScreenButtonAtlasName;
            yield return GraphicResources.TutorialUIAtlasName;
            yield return GraphicResources.EndGameAtlasName;
            yield return GraphicResources.FaceAtlasName;
            yield return GraphicResources.ChocographAtlasName;
            yield return GraphicResources.MovieGalleryAtlasName;
            yield return GraphicResources.EndingTextUsJpGrAtlasName;
        }
    }
}