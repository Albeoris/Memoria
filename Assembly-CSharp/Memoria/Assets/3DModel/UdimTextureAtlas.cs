using System;
using UnityEngine;

namespace Memoria.Assets
{
    public static class UdimTextureAtlas
    {
        public static Boolean IsTemplatePath(String texturePath)
        {
            return FbxUdimTexture.IsUdimPath(texturePath);
        }

        /// <summary>Attempts to build an atlas to allow textures to be used as UDIM tiles.</summary>
        public static Boolean TryBuild(String defaultFolder, String templatePath, out Texture2D atlas, out String atlasKey, out String error)
        {
            atlas = null;
            atlasKey = null;
            error = null;

            if (!IsTemplatePath(templatePath))
            {
                error = "the texture path must contain a <UDIM> placeholder";
                return false;
            }
            if (!FbxUdimTexture.TryResolvePath(defaultFolder, templatePath, out String safeTexturePath, out _, out error))
                return false;
            if (!FbxUdimTexture.TryCreate(safeTexturePath, out FbxUdimTexture udimTexture, out error))
                return false;

            try
            {
                atlas = udimTexture.TakeTexture();
                if (atlas == null)
                {
                    error = "the UDIM atlas could not be created";
                    return false;
                }
                atlasKey = atlas.name;
                return true;
            }
            finally
            {
                udimTexture.Destroy();
            }
        }
    }
}
