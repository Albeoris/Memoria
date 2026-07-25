using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Memoria.Assets
{
    internal sealed class FbxUdimTexture
    {
        public const String Placeholder = "<UDIM>";

        private const Int32 FirstTileNumber = 1001;
        private const Int32 TilesPerRow = 10;
        private const Int32 GutterSize = 2;
        private const String SafePathPlaceholder = "__MemoriaUdimPlaceholder__";

        public Texture2D Texture { get; private set; }

        private FbxUdimTexture(Texture2D texture, Int32 tileWidth, Int32 tileHeight, Int32 columnCount, Int32 rowCount)
        {
            Texture = texture;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _columnCount = columnCount;
            _rowCount = rowCount;
        }

        public static Boolean IsUdimPath(String path)
        {
            return !String.IsNullOrEmpty(path) && path.IndexOf(Placeholder, StringComparison.Ordinal) >= 0;
        }

        public static Boolean TryResolvePath(String defaultFolder, String texturePath, out String safeTexturePath, out String displayTexturePath, out String error)
        {
            safeTexturePath = null;
            displayTexturePath = texturePath;
            error = null;
            Int32 placeholderIndex = texturePath.IndexOf(Placeholder, StringComparison.Ordinal);
            if (placeholderIndex < 0 || texturePath.IndexOf(Placeholder, placeholderIndex + Placeholder.Length, StringComparison.Ordinal) >= 0)
            {
                error = "the texture path must contain exactly one <UDIM> placeholder";
                return false;
            }
            if (texturePath.IndexOf(SafePathPlaceholder, StringComparison.Ordinal) >= 0)
            {
                error = "the texture path contains a reserved UDIM path token";
                return false;
            }

            try
            {
                String pathWithoutIllegalCharacters = texturePath.Replace(Placeholder, SafePathPlaceholder);
                safeTexturePath = pathWithoutIllegalCharacters.Contains("/") ? pathWithoutIllegalCharacters : Path.Combine(defaultFolder, pathWithoutIllegalCharacters);
                displayTexturePath = safeTexturePath.Replace(SafePathPlaceholder, Placeholder);
                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
            }
        }

        public static Boolean TryCreate(String safeTexturePath, out FbxUdimTexture result, out String error)
        {
            result = null;
            error = null;
            List<UdimTile> tiles = new List<UdimTile>();
            Texture2D atlas = null;

            try
            {
                String folderPath = Path.GetDirectoryName(safeTexturePath);
                String fileName = Path.GetFileName(safeTexturePath);
                Int32 placeholderIndex = fileName.IndexOf(SafePathPlaceholder, StringComparison.Ordinal);
                if (placeholderIndex < 0 || fileName.IndexOf(SafePathPlaceholder, placeholderIndex + SafePathPlaceholder.Length, StringComparison.Ordinal) >= 0)
                {
                    error = "the texture path must contain exactly one <UDIM> placeholder";
                    return false;
                }
                if (String.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                {
                    error = $"the texture folder '{folderPath}' does not exist";
                    return false;
                }

                String prefix = fileName.Substring(0, placeholderIndex);
                String suffix = fileName.Substring(placeholderIndex + SafePathPlaceholder.Length);
                Dictionary<Int32, String> tilePaths = new Dictionary<Int32, String>();
                foreach (String candidatePath in Directory.GetFiles(folderPath))
                {
                    String candidateName = Path.GetFileName(candidatePath);
                    if (candidateName.Length != prefix.Length + 4 + suffix.Length ||
                        !candidateName.StartsWith(prefix, StringComparison.Ordinal) ||
                        !candidateName.EndsWith(suffix, StringComparison.Ordinal))
                        continue;

                    String numberText = candidateName.Substring(prefix.Length, 4);
                    if (!Int32.TryParse(numberText, out Int32 tileNumber) || tileNumber < FirstTileNumber)
                        continue;
                    if (tilePaths.ContainsKey(tileNumber))
                    {
                        error = $"more than one file matches UDIM tile {tileNumber}";
                        return false;
                    }
                    tilePaths.Add(tileNumber, candidatePath);
                }
                if (tilePaths.Count == 0)
                {
                    error = "no numbered UDIM tiles were found";
                    return false;
                }
                if (!tilePaths.ContainsKey(FirstTileNumber))
                {
                    error = "UDIM tile 1001 is missing";
                    return false;
                }

                Int32 maxColumn = 0;
                Int32 maxRow = 0;
                foreach (KeyValuePair<Int32, String> pair in tilePaths)
                {
                    Int32 tileIndex = pair.Key - FirstTileNumber;
                    maxColumn = Math.Max(maxColumn, tileIndex % TilesPerRow);
                    maxRow = Math.Max(maxRow, tileIndex / TilesPerRow);
                }
                Int32 columnCount = maxColumn + 1;
                Int32 rowCount = maxRow + 1;
                for (Int32 row = 0; row < rowCount; row++)
                {
                    for (Int32 column = 0; column < columnCount; column++)
                    {
                        Int32 tileNumber = FirstTileNumber + row * TilesPerRow + column;
                        if (!tilePaths.ContainsKey(tileNumber))
                        {
                            error = $"UDIM tile {tileNumber} is missing; tiles must form a rectangle starting at 1001";
                            return false;
                        }
                    }
                }

                Int32 tileWidth = 0;
                Int32 tileHeight = 0;
                List<Int32> tileNumbers = new List<Int32>(tilePaths.Keys);
                tileNumbers.Sort();
                foreach (Int32 tileNumber in tileNumbers)
                {
                    Byte[] raw = File.ReadAllBytes(tilePaths[tileNumber]);
                    Texture2D tileTexture = AssetManager.LoadTextureGeneric(raw);
                    if (tileTexture == null)
                    {
                        error = $"UDIM tile {tileNumber} is not a supported image";
                        return false;
                    }
                    tiles.Add(new UdimTile(tileNumber, tileTexture));
                    if (tileWidth == 0)
                    {
                        tileWidth = tileTexture.width;
                        tileHeight = tileTexture.height;
                    }
                    else if (tileTexture.width != tileWidth || tileTexture.height != tileHeight)
                    {
                        Int32 incompatibleWidth = tileTexture.width;
                        Int32 incompatibleHeight = tileTexture.height;
                        error = $"UDIM tile {tileNumber} is {incompatibleWidth}x{incompatibleHeight}; expected {tileWidth}x{tileHeight}";
                        return false;
                    }
                }

                Int32 cellWidth = checked(tileWidth + GutterSize * 2);
                Int32 cellHeight = checked(tileHeight + GutterSize * 2);
                Int32 atlasWidth = checked(cellWidth * columnCount);
                Int32 atlasHeight = checked(cellHeight * rowCount);
                Int32 maximumTextureSize = SystemInfo.maxTextureSize;
                if (maximumTextureSize > 0 && (atlasWidth > maximumTextureSize || atlasHeight > maximumTextureSize))
                {
                    error = $"the {atlasWidth}x{atlasHeight} UDIM atlas exceeds the maximum texture size {maximumTextureSize}";
                    return false;
                }

                Color32[] atlasPixels = new Color32[checked(atlasWidth * atlasHeight)];
                foreach (UdimTile tile in tiles)
                {
                    Int32 tileIndex = tile.Number - FirstTileNumber;
                    Int32 column = tileIndex % TilesPerRow;
                    Int32 row = tileIndex / TilesPerRow;
                    Int32 originX = column * cellWidth + GutterSize;
                    Int32 originY = row * cellHeight + GutterSize;
                    Color32[] tilePixels = tile.Texture.GetPixels32();

                    // Copies the tile and repeats its edge pixels into the gutter.
                    for (Int32 y = -GutterSize; y < tileHeight + GutterSize; y++)
                    {
                        Int32 sourceY = Math.Max(0, Math.Min(tileHeight - 1, y));
                        Int32 targetY = originY + y;
                        for (Int32 x = -GutterSize; x < tileWidth + GutterSize; x++)
                        {
                            Int32 sourceX = Math.Max(0, Math.Min(tileWidth - 1, x));
                            Int32 targetX = originX + x;
                            atlasPixels[targetY * atlasWidth + targetX] = tilePixels[sourceY * tileWidth + sourceX];
                        }
                    }
                }

                atlas = new Texture2D(atlasWidth, atlasHeight, AssetManager.DefaultTextureFormat, false);
                atlas.name = GetSafeTextureName(fileName);
                atlas.wrapMode = TextureWrapMode.Clamp;
                atlas.SetPixels32(atlasPixels);
                atlas.Apply();
                result = new FbxUdimTexture(atlas, tileWidth, tileHeight, columnCount, rowCount);
                atlas = null;
                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
            }
            finally
            {
                foreach (UdimTile tile in tiles)
                    UnityEngine.Object.Destroy(tile.Texture);
                if (atlas != null)
                    UnityEngine.Object.Destroy(atlas);
            }
        }

        public Boolean TryRemapUVs(Vector2[] uvs, out String error)
        {
            error = null;
            if (uvs == null)
            {
                error = "the mesh has no UV coordinates";
                return false;
            }

            for (Int32 i = 0; i < uvs.Length; i++)
            {
                Vector2 uv = uvs[i];
                if (!TryGetTileCoordinate(uv.x, _columnCount, out Int32 column, out Single localU) ||
                    !TryGetTileCoordinate(uv.y, _rowCount, out Int32 row, out Single localV))
                {
                    error = $"UV {i} ({uv.x}, {uv.y}) is outside the discovered {_columnCount}x{_rowCount} UDIM layout";
                    return false;
                }
                uvs[i] = RemapUV(column, row, localU, localV);
            }
            return true;
        }

        public Boolean ContainsTile(Int32 tileNumber)
        {
            return TryGetTilePosition(tileNumber, out _, out _);
        }

        public Boolean TryPrepareBlinkUVs(Vector2[] sourceUVs, Int32 openTileNumber, Int32 closedTileNumber, out Vector2[] openUVs, out Vector2[] closedUVs, out Int32 blinkVertexCount, out String error)
        {
            openUVs = null;
            closedUVs = null;
            blinkVertexCount = 0;
            error = null;
            if (sourceUVs == null)
            {
                error = "the mesh has no UV coordinates";
                return false;
            }
            if (!TryGetTilePosition(openTileNumber, out Int32 openColumn, out Int32 openRow))
            {
                error = $"open tile {openTileNumber} is unavailable in the discovered UDIM layout";
                return false;
            }
            if (!TryGetTilePosition(closedTileNumber, out _, out _))
            {
                error = $"closed tile {closedTileNumber} is unavailable in the discovered UDIM layout";
                return false;
            }

            openUVs = new Vector2[sourceUVs.Length];
            closedUVs = new Vector2[sourceUVs.Length];
            for (Int32 i = 0; i < sourceUVs.Length; i++)
            {
                Vector2 sourceUV = sourceUVs[i];
                if (!TryGetTileCoordinate(sourceUV.x, _columnCount, out Int32 column, out Single localU) ||
                    !TryGetTileCoordinate(sourceUV.y, _rowCount, out Int32 row, out Single localV))
                {
                    error = $"UV {i} ({sourceUV.x}, {sourceUV.y}) is outside the discovered {_columnCount}x{_rowCount} UDIM layout";
                    openUVs = null;
                    closedUVs = null;
                    blinkVertexCount = 0;
                    return false;
                }

                Vector2 atlasUV = RemapUV(column, row, localU, localV);
                closedUVs[i] = atlasUV;
                if (GetTileNumber(column, row) == closedTileNumber)
                {
                    openUVs[i] = RemapUV(openColumn, openRow, localU, localV);
                    blinkVertexCount++;
                }
                else
                {
                    openUVs[i] = atlasUV;
                }
            }
            return true;
        }

        public void Destroy()
        {
            if (Texture != null)
            {
                UnityEngine.Object.Destroy(Texture);
                Texture = null;
            }
        }

        internal Texture2D TakeTexture()
        {
            Texture2D texture = Texture;
            Texture = null;
            return texture;
        }

        private static Boolean TryGetTileCoordinate(Single value, Int32 tileCount, out Int32 coordinate, out Single localValue)
        {
            if (Single.IsNaN(value) || Single.IsInfinity(value))
            {
                coordinate = -1;
                localValue = 0f;
                return false;
            }
            coordinate = Mathf.FloorToInt(value);
            localValue = value - coordinate;
            if (coordinate == tileCount && Mathf.Approximately(localValue, 0f))
            {
                coordinate--;
                localValue = 1f;
            }
            return coordinate >= 0 && coordinate < tileCount;
        }

        private Boolean TryGetTilePosition(Int32 tileNumber, out Int32 column, out Int32 row)
        {
            Int32 tileIndex = tileNumber - FirstTileNumber;
            if (tileIndex < 0)
            {
                column = -1;
                row = -1;
                return false;
            }
            column = tileIndex % TilesPerRow;
            row = tileIndex / TilesPerRow;
            return column < _columnCount && row < _rowCount;
        }

        private static Int32 GetTileNumber(Int32 column, Int32 row)
        {
            return FirstTileNumber + row * TilesPerRow + column;
        }

        private Vector2 RemapUV(Int32 column, Int32 row, Single localU, Single localV)
        {
            Single cellWidth = _tileWidth + GutterSize * 2;
            Single cellHeight = _tileHeight + GutterSize * 2;
            Single atlasWidth = cellWidth * _columnCount;
            Single atlasHeight = cellHeight * _rowCount;
            return new Vector2(
                (column * cellWidth + GutterSize + localU * _tileWidth) / atlasWidth,
                (row * cellHeight + GutterSize + localV * _tileHeight) / atlasHeight);
        }

        private static String GetSafeTextureName(String fileName)
        {
            String name = Path.GetFileNameWithoutExtension(fileName).Replace(SafePathPlaceholder, String.Empty).TrimEnd('.', '-', '_', ' ');
            return String.IsNullOrEmpty(name) ? "UDIMAtlas" : name + "_UDIMAtlas";
        }

        private sealed class UdimTile
        {
            public readonly Int32 Number;
            public readonly Texture2D Texture;

            public UdimTile(Int32 number, Texture2D texture)
            {
                Number = number;
                Texture = texture;
            }
        }

        private readonly Int32 _tileWidth;
        private readonly Int32 _tileHeight;
        private readonly Int32 _columnCount;
        private readonly Int32 _rowCount;
    }
}
