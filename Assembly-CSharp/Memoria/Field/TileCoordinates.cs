using System;

namespace Memoria.Field
{
	public struct TileCoordinates
	{
		public readonly TileMap TileMap;
		public readonly Int32 HorizontalIndex;
		public readonly Int32 VerticalIndex;

		public TileCoordinates(TileMap tileMap, Int32 horizontalIndex, Int32 verticalIndex)
		{
			TileMap = tileMap;
			HorizontalIndex = horizontalIndex;
			VerticalIndex = verticalIndex;
		}

		public Boolean IsExists => TileMap.IsExists(HorizontalIndex, VerticalIndex);

		public Int32 TileSize => TileMap.TileSize;

		public TileCoordinates Left => TileMap.GetByIndices(HorizontalIndex - 1, VerticalIndex);
		public TileCoordinates Right => TileMap.GetByIndices(HorizontalIndex + 1, VerticalIndex);
		public TileCoordinates Top => TileMap.GetByIndices(HorizontalIndex, VerticalIndex - 1); // Swap?
		public TileCoordinates Bottom => TileMap.GetByIndices(HorizontalIndex, VerticalIndex + 1); // Swap?
		public TileCoordinates UpperLeft => TileMap.GetByIndices(HorizontalIndex - 1, VerticalIndex - 1); // Swap?
		public TileCoordinates UpperRight => TileMap.GetByIndices(HorizontalIndex + 1, VerticalIndex - 1); // Swap?
		public TileCoordinates BottomLeft => TileMap.GetByIndices(HorizontalIndex - 1, VerticalIndex + 1); // Swap?
		public TileCoordinates BottomRight => TileMap.GetByIndices(HorizontalIndex + 1, VerticalIndex + 1); // Swap?

		public Int32 LeftX => HorizontalIndex * TileSize;
		public Int32 RightX => LeftX + TileSize - 1;
		public Int32 TopY => VerticalIndex * TileSize; // Swap?
		public Int32 BottomY => TopY + TileSize - 1; // Swap?
	}
}
