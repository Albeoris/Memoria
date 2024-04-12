using System;

namespace Memoria.Field
{
	public sealed class TileMap
	{
		public readonly Int32 TextureWidth;
		public readonly Int32 TextureHeight;
		public readonly Int32 TileSize;

		public readonly Int32 TilePerRow;
		public readonly Int32 TilePerCol;

		public TileMap(Int32 textureWidth, Int32 textureHeight, Int32 tileSize)
		{
			TextureWidth = textureWidth;
			TextureHeight = textureHeight;
			TileSize = tileSize;

			TilePerRow = textureWidth / tileSize;
			TilePerCol = textureHeight / tileSize;

			if (textureWidth % tileSize != 0)
				throw new NotSupportedException($"textureWidth ({textureWidth}) % tileSize ({tileSize}) != 0");
			if (textureHeight % tileSize != 0)
				throw new NotSupportedException($"textureHeight ({textureHeight}) % tileSize ({tileSize}) != 0");
		}

		public TileCoordinates GetByCoordinates(Int32 pixelX, Int32 pixelY)
		{
			Int32 hIndex = pixelX / TileSize;
			Int32 vIndex = pixelY / TileSize;
			return GetByIndices(hIndex, vIndex);
		}

		public TileCoordinates GetByIndex(Int32 index)
		{
			Int32 hIndex = index % TilePerRow;
			Int32 vIndex = index / TilePerRow;
			return GetByIndices(hIndex, vIndex);
		}

		public TileCoordinates GetByIndices(Int32 hIndex, Int32 vIndex)
		{
			return new TileCoordinates(this, hIndex, vIndex);
		}

		public Boolean IsExists(Int32 horizontalIndex, Int32 verticalIndex)
		{
			return horizontalIndex >= 0 && horizontalIndex < TilePerRow
				   && verticalIndex >= 0 && verticalIndex < TilePerCol;
		}
	}
}
