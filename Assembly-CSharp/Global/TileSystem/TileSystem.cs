using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memoria.Prime;
using Memoria;

using Memoria.Prime.PsdFile;

using UnityEngine;
using Memoria.Assets;

namespace Global.TileSystem
{
    public struct Tile
    {
        public int x;
        public int y;
        public BGSPRITE_LOC_DEF info;
        public Overlay overlay;
        public bool empty;
    }

    struct Size
    {
        public int height;
        public int width;
    }

    struct Point
    {
        public int x;
        public int y;
        public Overlay overlay;
    }

    class CopyBytesHelper
    {
        private readonly uint _factor;
        public readonly Size size;
        public readonly uint _tileWidth;
        public readonly uint _tileHeight;

        public bool PaddingNeeded(Tile tile, Overlay overlay, PaddingType paddingType)
        {
            uint fromIndexX = tile.info.offX * _factor;
            uint trueHeight = overlay.info.h * _factor;
            uint trueWidth = overlay.info.w * _factor;
            uint trueOffsetY = tile.info.offY * _factor;
            uint fromIndexY = trueHeight - (trueOffsetY + Convert.ToUInt32(_tileHeight)); // sigh

            // for debug
            var ind = overlay.GetOrderNumber();
            //if (ind == 20)
            //    return false;

            //if (overlay.GetOrderNumber() == 29)
            //    return false;
            uint cornerPixelIndex;
            switch (paddingType)
            {
                case PaddingType.Left:
                    uint plusX = 0;
                    uint plusY = 0;
                    for (uint y = 0; y < _tileHeight; y++)
                    {
                        uint targetPixelIndex = (fromIndexY + y) * trueWidth + fromIndexX + plusX;
                        if (overlay.imageData[targetPixelIndex * 4 + 3] > 0)
                            return true;
                    }
                    return false;
                case PaddingType.Right:
                    plusX = _tileWidth - 1;
                    for (uint y = 0; y < _tileHeight; y++)
                    {
                        uint targetPixelIndex = (fromIndexY + y) * trueWidth + fromIndexX + plusX;
                        if (overlay.imageData[targetPixelIndex * 4 + 3] > 0)
                            return true;
                    }
                    return false;
                case PaddingType.Down:
                    plusY = _tileHeight - 1;
                    for (uint x = 0; x < _tileWidth; x++)
                    {
                        uint targetPixelIndex = (fromIndexY + plusY) * trueWidth + fromIndexX + x;
                        if (overlay.imageData[targetPixelIndex * 4 + 3] > 0)
                            return true;
                    }
                    return false;
                case PaddingType.Up:
                    plusY = 0;
                    for (uint x = 0; x < _tileWidth; x++)
                    {
                        uint targetPixelIndex = (fromIndexY + plusY) * trueWidth + fromIndexX + x;
                        if (overlay.imageData[targetPixelIndex * 4 + 3] > 0)
                            return true;
                    }
                    return false;
                case PaddingType.DownLeft:
                    plusY = _tileHeight - 1;
                    plusX = 0;
                    cornerPixelIndex = (fromIndexY + plusY) * trueWidth + fromIndexX + plusX;
                    if (overlay.imageData[cornerPixelIndex * 4 + 3] > 0)
                        return true;
                    return false;
                case PaddingType.DownRight:
                    plusY = _tileHeight - 1;
                    plusX = _tileWidth - 1;
                    cornerPixelIndex = (fromIndexY + plusY) * trueWidth + fromIndexX + plusX;
                    if (overlay.imageData[cornerPixelIndex * 4 + 3] > 0)
                        return true;
                    return false;
                case PaddingType.UpLeft:
                    plusY = 0;
                    plusX = 0;
                    cornerPixelIndex = (fromIndexY + plusY) * trueWidth + fromIndexX + plusX;
                    if (overlay.imageData[cornerPixelIndex * 4 + 3] > 0)
                        return true;
                    return false;
                case PaddingType.UpRight:
                    plusY = 0;
                    plusX = _tileWidth - 1;
                    cornerPixelIndex = (fromIndexY + plusY) * trueWidth + fromIndexX + plusX;
                    if (overlay.imageData[cornerPixelIndex * 4 + 3] > 0)
                        return true;
                    return false;
            }
            return true;
        }

        public CopyBytesHelper(uint factor, uint tileWidth, uint tileHeight)
        {
            _factor = factor;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            size = new Size();
            size.height = (int)tileHeight;
            size.width = (int)tileWidth;
        }

        public void FillBackgroundOverlays(TileMap tilemap)
        {
            TileDepthMap tileDepthMap = tilemap.TDMap;

            for (var i = 0; i < tilemap.SizeY; i++)
            {
                for (var j = 0; j < tilemap.SizeX; j++)
                {
                    List<TileDepthInfo> lst = tileDepthMap.TileMap[j, i];
                    if (lst == null || lst.Count < 2)
                        continue;
                    int overlayNum = lst[0].orderNumber;
                    Overlay targetOverlay = tilemap.GetOverlay(overlayNum);
                    Tile targetTile = targetOverlay.GetTile(j, i);
                    for (var k = 1; k < lst.Count; k++)
                    {
                        var toX = (int)(targetTile.info.offX * _factor);
                        var toY = (int)(targetOverlay.info.h * this._factor - (targetTile.info.offY * this._factor + this._tileHeight));
                        Overlay sourceOverlay = tilemap.GetOverlay(lst[k].orderNumber);
                        Tile sourceTile = sourceOverlay.GetTile(j, i);
                        this.CopyTile(targetOverlay.imageData, targetOverlay.info.w * (int)_factor, toX, toY, sourceTile, sourceOverlay, true);
                    }
                }
            }
        }

        public void CopyPaddingByPixels(byte[] atlasArray, int atlasWidth, int atlasX, int atlasY, Padding padding)
        {
            var leftBoundary = atlasX - 1;
            var rightBoundary = atlasX + (int)_tileWidth;
            var bottomBoundary = atlasY - 1;
            var topBoundary = atlasY + (int)_tileHeight;

            Size rectSize = new Size
            {
                height = 1,
                width = 1
            };
            int toX = leftBoundary;
            int toY = bottomBoundary;

            switch (padding.type)
            {
                case PaddingType.Left:
                    rectSize.height = (int)_tileHeight;
                    rectSize.width = 1;
                    toX = leftBoundary;
                    toY = atlasY;
                    break;
                case PaddingType.Right:
                    rectSize.height = (int)_tileHeight;
                    rectSize.width = 1;
                    toX = rightBoundary;
                    toY = atlasY;
                    break;
                case PaddingType.Down:
                    rectSize.height = 1;
                    rectSize.width = (int)_tileWidth;
                    toX = atlasX;
                    toY = topBoundary;
                    break;
                case PaddingType.Up:
                    rectSize.height = 1;
                    rectSize.width = (int)_tileWidth;
                    toX = atlasX;
                    toY = bottomBoundary;
                    break;
                case PaddingType.DownLeft:
                    rectSize.height = 1;
                    rectSize.width = 1;
                    toX = leftBoundary;
                    toY = topBoundary;
                    break;
                case PaddingType.DownRight:
                    rectSize.height = 1;
                    rectSize.width = 1;
                    toX = rightBoundary;
                    toY = topBoundary;
                    break;
                case PaddingType.UpLeft:
                    rectSize.height = 1;
                    rectSize.width = 1;
                    toX = leftBoundary;
                    toY = bottomBoundary;
                    break;
                case PaddingType.UpRight:
                    rectSize.height = 1;
                    rectSize.width = 1;
                    toX = rightBoundary;
                    toY = bottomBoundary;
                    break;
            }

            byte[] originalStrip = new byte[rectSize.height * rectSize.width * 4];
            uint originalFromX = ((uint)padding.paddingCoords.originalTileX + _tileWidth) % _tileWidth;
            uint originalFromY = ((uint)padding.paddingCoords.originalTileY + _tileHeight) % _tileHeight;
            this.CopyRect(originalStrip, rectSize.width, 0, 0, padding.GetOriginalTile(), padding.GetOriginalOverlay(), rectSize, originalFromX, originalFromY, false);

            // copy rect to sample rect from all overlays with transparency ignorance
            byte[] paddingStrip = new byte[rectSize.height * rectSize.width * 4];
            List<Overlay> candidateOverlays = padding.GetOverlayList();

            foreach (Overlay paddingOverlay in candidateOverlays)
            {
                Tile paddingTile = paddingOverlay.GetTile(padding.paddingCoords.tileCol, padding.paddingCoords.tileRow);
                uint paddingFromX = ((uint)padding.paddingCoords.ox + _tileWidth) % _tileWidth;
                uint paddingFromY = ((uint)padding.paddingCoords.oy + _tileHeight) % _tileHeight;
                this.CopyRectExp(paddingStrip, rectSize.width, 0, 0, paddingTile, paddingOverlay, rectSize, paddingFromX, paddingFromY, true);
                //break;
            }
            if (!padding.paddingCoords.selfPadding)
            {
                Tile selfPaddingTile = padding.GetOriginalTile();
                uint paddingFromX = ((uint)padding.paddingCoordsSelf.ox + _tileWidth) % _tileWidth;
                uint paddingFromY = ((uint)padding.paddingCoordsSelf.oy + _tileHeight) % _tileHeight;
                this.CopyRectExp(paddingStrip, rectSize.width, 0, 0, selfPaddingTile, padding.GetOriginalOverlay(), rectSize, paddingFromX, paddingFromY, true);
            }


            // mask by originalStrip
            for (int i = 0; i < paddingStrip.Length; i += 4)
            {
                paddingStrip[i + 3] = originalStrip[i + 3];
            }


            var horizontal = (padding.type == PaddingType.Left || padding.type == PaddingType.Right) ? 1 : 0;
            var vertical = (padding.type == PaddingType.Up || padding.type == PaddingType.Down) ? 1 : 0;
            var horDirection = padding.type == PaddingType.Left ? -1 : 1;
            var verDirection = padding.type == PaddingType.Up ? -1 : 1;
            for (var i = 0; i < _factor; i++) // _factor
            {
                CopyStripToAtlas(atlasArray, atlasWidth,
                    toX + horizontal * horDirection * i, toY + vertical * verDirection * i,
                    paddingStrip, rectSize
                    );
            }

        }


        public void CopyTile(byte[] atlasArray, int atlasWidth, int toX, int toY, Tile tile, Overlay overlay, bool ignoreTransparent)
        {
            Size rectSize = this.size;
            CopyRect(atlasArray, atlasWidth, toX, toY, tile, overlay, rectSize, 0, 0, ignoreTransparent);
        }

        public void CopyStripToAtlas(byte[] atlasArray, int atlasWidth, int atlasX, int atlasY, byte[] stripData, Size stripSize)
        {
            uint k = 0;
            for (uint i = 0; i < stripSize.height; i++)
            {
                for (uint j = 0; j < stripSize.width; j++)
                {
                    uint toIndex = (uint)((atlasY + i) * atlasWidth + atlasX + j);
                    CopyBytesHelper.CopyPixel(stripData, atlasArray, k++, toIndex);
                }
            }
        }

        public void CopyRect(byte[] atlasArray, int atlasWidth, int toX, int toY, Tile tile, Overlay overlay, Size rectSize, uint fromX, uint fromY, bool ignoreTransparent)
        {
            if (tile.info == null)
            {
                Log.Warning($"MISSING TILE {overlay.GetOrderNumber()}, ({tile.x}, {tile.y})");
                return;
            }
            uint fromIndexX = tile.info.offX * _factor + fromX;
            uint trueHeight = overlay.info.h * _factor;
            uint trueWidth = overlay.info.w * _factor;
            uint trueOffsetY = tile.info.offY * _factor;
            uint fromIndexY = trueHeight - (trueOffsetY - fromY + Convert.ToUInt32(this.size.height)); // sigh
            for (uint i = 0; i < rectSize.height; i++)
            {
                for (uint j = 0; j < rectSize.width; j++)
                {
                    uint fromIndex = (fromIndexY + i) * trueWidth + fromIndexX + j;
                    uint toIndex = (uint)((toY + i) * atlasWidth + toX + j);
                    try
                    {
                        if (ignoreTransparent && overlay.imageData[fromIndex * 4 + 3] < 255)
                            continue;
                        CopyBytesHelper.CopyPixel(overlay.imageData, atlasArray, fromIndex, toIndex);
                    }
                    catch
                    {
                        Log.Message($"trueHeight {trueHeight}, trueOffsetY {trueOffsetY}, fromY {fromY}, size.height {this.size.height}, difference {trueOffsetY - fromY + Convert.ToUInt32(this.size.height)}");
                        Log.Message($"trueWidth {trueWidth}, fromIndexY {fromIndexY}, fromIndexX {fromIndexX}");
                        Log.Message($"fromIndex {fromIndex}, imageData {overlay.imageData.Length} toIndex{toIndex}, atlas {atlasArray.Length}");
                        return;
                    }
                }
            }
        }

        public void CopyRectExp(byte[] atlasArray, int atlasWidth, int toX, int toY, Tile tile, Overlay overlay, Size rectSize, uint fromX, uint fromY, bool ignoreTransparent)
        {
            if (tile.info == null)
            {
                Log.Warning($"MISSING TILE {overlay.GetOrderNumber()}, ({tile.x}, {tile.y}) COPY RECT EXP");
                return;
            }
            uint fromIndexX = tile.info.offX * _factor + fromX;
            uint trueHeight = overlay.info.h * _factor;
            uint trueWidth = overlay.info.w * _factor;
            uint trueOffsetY = tile.info.offY * _factor;
            uint fromIndexY = trueHeight - (trueOffsetY - fromY + Convert.ToUInt32(this.size.height)); // sigh
            for (uint i = 0; i < rectSize.height; i++)
            {
                for (uint j = 0; j < rectSize.width; j++)
                {
                    uint fromIndex = (fromIndexY + i) * trueWidth + fromIndexX + j;
                    uint toIndex = (uint)((toY + i) * atlasWidth + toX + j);
                    if (ignoreTransparent && overlay.imageData[fromIndex * 4 + 3] < 127)
                        continue;
                    CopyBytesHelper.CopyPixel(overlay.imageData, atlasArray, fromIndex, toIndex);
                }
            }
        }

        private static void CopyPixel(byte[] from, byte[] to, uint fromIndex, uint toIndex)
        {
            try
            {
                if (to[toIndex * 4 + 3] > 127)
                    return;
                for (var i = 0; i < 4; i++)
                {
                    to[toIndex * 4 + i] = from[fromIndex * 4 + i];
                }
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
        }

        public static byte[] GetImageData(Layer layer, BGOVERLAY_DEF overlayInfo, uint factor)
        {
            int SPRITE_H = 16 * (int)factor;
            int SPRITE_W = 16 * (int)factor;

            long overlayHeight = overlayInfo.h * factor;
            long overlayWidth = overlayInfo.w * factor;

            Rectangle rect = layer.Rect;
            long left = overlayInfo.curX;
            long right = left + overlayInfo.w;
            long top = overlayInfo.curY;
            long bottom = top + overlayInfo.h;
            long product = overlayHeight * overlayWidth;
            long verticalOffset = (SPRITE_H - rect.Bottom % SPRITE_H) % SPRITE_H;
            long horizontalOffset = rect.Left % SPRITE_W;
            Rectangle currentRect = layer.Rect;
            // fill overlay with transparency

            byte[] layerbytes = new byte[product * 4];
            for (var i = 0; i < product * 4; i++)
            {
                if (i % 4 == 3)
                    layerbytes[i] = 0;
                layerbytes[i] = 1;
            }

            Int32 width = rect.Width, height = rect.Height;
            Int32 initialCorrection = (Int32)(overlayWidth * (verticalOffset) + horizontalOffset);
            long initialX = currentRect.Left - left * factor;
            long initialY = bottom * factor - currentRect.Bottom;
            foreach (var channel in layer.Channels)
            {

                byte[] imgdata = channel.ImageData;
                Int32 offset = channel.ID;
                if (offset == -1) offset = 3;
                // rows are flipped vertically in 
                for (var y = 0; y < rect.Height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {

                        var dstCoordX = x + initialX;
                        if (dstCoordX < 0 || dstCoordX > overlayWidth - 1) continue;
                        var dstCoordY = rect.Height - y - 1 + initialY;
                        if (dstCoordY < 0 || dstCoordY > overlayHeight - 1) continue;
                        var dstIndex = (dstCoordY * overlayWidth + dstCoordX) * 4;
                        layerbytes[dstIndex + offset] = imgdata[x + y * width];
                    }
                }
            }
            return layerbytes;
        }
    }

    public struct PaddingCoords
    {
        public int tileCol;
        public int tileRow;
        public int ox;
        public int oy;
        public int originalTileX;
        public int originalTileY;
        public bool selfPadding;
    }

    public struct TileDepthInfo
    {
        public int depth;
        public int orderNumber;
    }

    class TileDepthMap
    {
        public List<TileDepthInfo>[,] TileMap;
        public int SizeX, SizeY;

        public TileDepthMap(Dictionary<int, Overlay> overlays, int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            TileMap = new List<TileDepthInfo>[sizeX, sizeY];
            foreach (KeyValuePair<Int32, Overlay> entry in overlays)
            {
                // exclude animations
                Overlay overlay = entry.Value;
                if (overlay.IsAnimation() || overlay.IsAlpha()) continue;
                for (var tileRow = 0; tileRow < sizeY; tileRow++)
                {
                    for (var tileCol = 0; tileCol < sizeX; tileCol++)
                    {
                        if (TileMap[tileCol, tileRow] == null)
                            TileMap[tileCol, tileRow] = new List<TileDepthInfo>();
                        Tile tile = overlay.GetTile(tileCol, tileRow);
                        if (tile.info != null && tile.info.alpha == 0)
                        {
                            TileMap[tileCol, tileRow].Add(new TileDepthInfo
                            {
                                depth = tile.info.depth + overlay.info.curZ,
                                orderNumber = overlay.GetOrderNumber()

                            });
                        }
                    }
                }
            }

            for (var tileRow = 0; tileRow < sizeY; tileRow++)
            {
                for (var tileCol = 0; tileCol < sizeX; tileCol++)
                {
                    if(TileMap[tileCol, tileRow] != null)
                        TileMap[tileCol, tileRow].Sort(delegate (TileDepthInfo x, TileDepthInfo y)
                        {
                            return y.depth.CompareTo(x.depth);
                        });
                }
            }
        }
    }

    class TileMap
    {
        private int _cameraIndex;
        private int _fieldMapNumber;
        private Dictionary<Int32, Overlay> _overlays;
        public int MinX;
        public int MinY;
        public int SizeX;
        public int SizeY;
        private List<BGANIM_DEF> _animList;
        private List<BGLIGHT_DEF> _lightList;
        public TileDepthMap TDMap;

        private PaddingCoords GetPaddingSourceCoords(Tile tile, PaddingType paddingType, bool forceSelf)
        {

            int tileCol, tileRow, ox, oy, backupX, backupY, originalTileX, originalTileY;
            bool selfPadding = false;

            switch (paddingType)
            {
                case PaddingType.Left:
                    tileCol = tile.x - 1;
                    tileRow = tile.y;
                    ox = -1;
                    oy = 0;
                    originalTileX = 0;
                    originalTileY = 0;
                    backupX = 0;
                    backupY = 0;
                    break;
                case PaddingType.Right:
                    tileCol = tile.x + 1;
                    tileRow = tile.y;
                    ox = 0;
                    oy = 0;
                    originalTileX = -1;
                    originalTileY = 0;
                    backupX = -1;
                    backupY = 0;
                    break;
                case PaddingType.Down:
                    tileCol = tile.x;
                    tileRow = tile.y - 1;
                    ox = 0;
                    oy = 0;
                    originalTileX = 0;
                    originalTileY = -1;
                    backupX = 0;
                    backupY = -1;
                    break;
                case PaddingType.Up:
                    tileCol = tile.x;
                    tileRow = tile.y + 1;
                    ox = 0;
                    oy = -1;
                    originalTileX = 0;
                    originalTileY = 0;
                    backupX = 0;
                    backupY = 0;
                    break;
                case PaddingType.DownLeft:
                    tileCol = tile.x - 1;
                    tileRow = tile.y - 1;
                    ox = -1;
                    oy = 0;
                    originalTileX = 0;
                    originalTileY = -1;
                    backupX = 0;
                    backupY = -1;
                    break;
                case PaddingType.DownRight:
                    tileCol = tile.x + 1;
                    tileRow = tile.y - 1;
                    ox = 0;
                    oy = 0;
                    originalTileX = -1;
                    originalTileY = -1;
                    backupX = -1;
                    backupY = -1;
                    break;
                case PaddingType.UpLeft:
                    tileCol = tile.x - 1;
                    tileRow = tile.y + 1;
                    ox = -1;
                    oy = -1;
                    originalTileX = 0;
                    originalTileY = 0;
                    backupX = 0;
                    backupY = 0;
                    break;
                case PaddingType.UpRight:
                    tileCol = tile.x - 1;
                    tileRow = tile.y + 1;
                    ox = 0;
                    oy = -1;
                    originalTileX = -1;
                    originalTileY = 0;
                    backupX = -1;
                    backupY = 0;
                    break;
                default:
                    tileCol = tile.x;
                    tileRow = tile.y;
                    ox = -1;
                    oy = -1;
                    originalTileX = 0;
                    originalTileY = 0;
                    backupX = 0;
                    backupY = 0;
                    break;
            }
            if (forceSelf || tileRow < 0 || tileRow >= this.SizeY ||
                tileCol < 0 || tileCol >= this.SizeX)
            //|| paddingType > PaddingType.Up)
            {
                tileCol = tile.x;
                tileRow = tile.y;
                ox = backupX;
                oy = backupY;
                selfPadding = true;
            }


            PaddingCoords paddingCoords = new PaddingCoords
            {
                tileCol = tileCol,
                tileRow = tileRow,
                ox = ox,
                oy = oy,
                originalTileX = originalTileX,
                originalTileY = originalTileY,
                selfPadding = selfPadding
            };
            return paddingCoords;
        }

        private int GetTileDepth(Tile tile, Overlay overlay)
        {
            return tile.info.depth + overlay.info.curZ;
        }

        private Overlay GetNearestSourceOverlayByDepth(Overlay overlay, Tile tile, int x, int y)
        {
            Overlay overlayToReturn = overlay;
            int desiredOverlayIndex = overlay.GetOrderNumber();
            int originalDepth = overlay.info.curZ + tile.info.depth;
            int difference = int.MaxValue;

            if (!overlay.IsAnimation() && !overlay.IsAlpha())
            {
                foreach (KeyValuePair<Int32, Overlay> entry in _overlays)
                {
                    Overlay currentOverlay = entry.Value;
                    int i = entry.Key;
                    if (currentOverlay.IsAnimation() || currentOverlay.IsAlpha()) continue;
                    Tile currentTile = currentOverlay.GetTile(x, y);
                    if (currentTile.info == null) continue;
                    int thisDepth = currentOverlay.info.curZ + currentTile.info.depth;
                    int candidateDifference = Math.Abs(thisDepth - originalDepth);
                    if (candidateDifference <= difference)
                    {
                        difference = candidateDifference;
                        desiredOverlayIndex = i;
                    }
                }
            }
            return this._overlays[desiredOverlayIndex];
        }

        private int _getOverlayToRemove(int overlayNumber, PaddingType paddingType, int x, int y)
        {
            if (_fieldMapNumber == 312 && paddingType == PaddingType.Down 
                && overlayNumber == 2 && x < 17 && y == 12)
            {
                return 5;
            }
            return -1;
        }

        public Padding GetPaddingForTile(PaddingType paddingType, Overlay overlay, int x, int y)
        {
            Tile tile = overlay.GetTile(x, y);

            List<Overlay> selfPaddingOverlayList = new List<Overlay>() { overlay };
            PaddingCoords paddingCoords = this.GetPaddingSourceCoords(tile, paddingType, false);
            List<TileDepthInfo> overlayNumbers = new List<TileDepthInfo>();

            if (paddingCoords.tileCol < this.TDMap.SizeX && paddingCoords.tileRow < this.TDMap.SizeY
                && this.TDMap.TileMap[paddingCoords.tileCol, paddingCoords.tileRow] != null)
            {
                overlayNumbers = new List<TileDepthInfo>(this.TDMap.TileMap[paddingCoords.tileCol, paddingCoords.tileRow]);
            }

            PaddingCoords selfCoords = this.GetPaddingSourceCoords(tile, paddingType, true);
            if (paddingCoords.selfPadding || overlayNumbers.Count < 1) // || overlay.IsAnimation() || overlay.IsAlpha())
            {
                // case closed let's get drunk
                return new Padding(tile, overlay, selfPaddingOverlayList, selfCoords, selfCoords, paddingType);
            }

            if (overlay.IsAlpha() || overlay.IsAnimation())
                return new Padding(tile, overlay, new List<Overlay>() { overlay }, paddingCoords, selfCoords, paddingType);

          
            int sourceDepth = overlay.info.curZ + tile.info.depth;
            List<Overlay> paddingOverlayList = new List<Overlay>();
            var currentNumber = overlay.GetOrderNumber();

            //int numberToRemove = _getOverlayToRemove(currentNumber, paddingType, x, y);
            //if (numberToRemove != -1)
            //{
            //    for (var i = 0; i < overlayNumbers.Count; i++)
            //    {
            //        var elem = overlayNumbers[i];
            //        if (elem.orderNumber == numberToRemove)
            //        {
            //            overlayNumbers.Remove(elem);
            //            break;
            //        }
            //    }
            //}
            // this one is original order
            //overlayNumbers.Sort(delegate (TileDepthInfo first, TileDepthInfo second)
            //{
            //    return (Math.Abs(first.depth - sourceDepth)).CompareTo(Math.Abs(second.depth - sourceDepth));
            //});

            // this one is not
            List<TileDepthInfo> greater = new List<TileDepthInfo>();
            List<TileDepthInfo> lesserOrEquals = new List<TileDepthInfo>();
            foreach(var element in overlayNumbers)
            {
                if (element.depth > sourceDepth)
                    greater.Add(element);
                else lesserOrEquals.Add(element);
            }
            greater.Reverse();
            overlayNumbers = lesserOrEquals.Concat(greater).ToList();

            

            for (int i = 0; i < overlayNumbers.Count; i++)
            {
                var item = overlayNumbers[i];
                if (overlayNumbers[i].orderNumber == currentNumber)
                {
                    if(i > 0)
                    {
                        overlayNumbers.RemoveAt(i);
                        overlayNumbers.Insert(0, item);
                    }
                    break;
                }
            }


           // String depths = "";
            for (var i = 0; i < overlayNumbers.Count; i++)
            {
                int number = overlayNumbers[i].orderNumber;
                paddingOverlayList.Add(this.GetOverlay(number));
               // depths += $"depth {overlayNumbers[i].depth}, orderNumber {overlayNumbers[i].orderNumber}\n ";
            }
           // Log.Message(depths);
            return new Padding(tile, overlay, paddingOverlayList, paddingCoords, selfCoords, paddingType);
        }


        public Overlay GetOverlay(int num)
        {
            try
            {
                return _overlays[num];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static void GetBounds(List<BGOVERLAY_DEF> overlayList, int camIndex, out Int32 minX, out Int32 minY, out Int32 maxX, out Int32 maxY)
        {
            maxX = Int32.MinValue;
            minX = Int32.MaxValue;
            maxY = Int32.MinValue;
            minY = Int32.MaxValue;
            for (int i = 0; i < overlayList.Count; i++)
            {

                BGOVERLAY_DEF overlay = overlayList[i];
                if (camIndex != overlay.camNdx)
                    continue;
                for (int k = 0; k < overlay.spriteList.Count; k++)
                {
                    BGSPRITE_LOC_DEF sprite = overlay.spriteList[k];
                    int offX = sprite.offX + overlay.curX;
                    int offY = sprite.offY + overlay.curY;
                    if (offX < minX) minX = offX;
                    if (offX > maxX) maxX = offX;
                    if (offY < minY) minY = offY;
                    if (offY > maxY) maxY = offY;
                }
            }
        }

        public TileMap(int fieldMapNumber, List<Layer> layers, List<BGOVERLAY_DEF> overlayList, List<BGANIM_DEF> animList, List<BGLIGHT_DEF> lightList, int cameraIndex, uint factor)
        {
            TileMap.GetBounds(overlayList, cameraIndex, out int minX, out int minY, out int maxX, out int maxY);
            this.MinX = minX;
            this.MinY = minY;
            this.SizeX = (maxX - minX) / 16 + 1;
            this.SizeY = (maxY - minY) / 16 + 1;
            _fieldMapNumber = fieldMapNumber;
            _cameraIndex = cameraIndex;
            _overlays = new Dictionary<int, Overlay>();
            _animList = animList;
            _lightList = lightList;

            bool[] animationArray = new bool[overlayList.Count];
            for (var m = 0; m < animList.Count; m++)
            {
                BGANIM_DEF animation = animList[m];
                for (var n = 0; n < animation.frameList.Count; n++)
                {
                    int index = animation.frameList[n].target;
                    animationArray[index] = true;
                }
            }
            for (var m = 0; m < lightList.Count; m++)
            {
                BGLIGHT_DEF lightDef = lightList[m];

            }
            for (int i = 0; i < overlayList.Count; i++)
            {
                BGOVERLAY_DEF info = overlayList[i];
                if (info.camNdx != _cameraIndex)
                    continue;
                Layer layer = layers[i];
                bool animationFlag = animationArray[i];
                Overlay overlay = new Overlay(CopyBytesHelper.GetImageData(layer, info, factor), info, i, this.SizeX, this.SizeY, minX, minY, maxX, maxY, animationFlag);
                _overlays[i] = overlay;
            }

            TDMap = new TileDepthMap(_overlays, this.SizeX, this.SizeY);
        }

        public TileMap(List<Layer> layers, List<BGOVERLAY_DEF> overlayList, List<BGANIM_DEF> animList, uint factor)
        {
            Int32 minX, minY, maxX, maxY;
            TileMap.GetBounds(overlayList, 0, out minX, out minY, out maxX, out maxY);
            this.SizeX = (maxX - minX) / 16;
            this.SizeY = (maxY - minY) / 16;
            _cameraIndex = overlayList[0].camNdx;
            _overlays = new Dictionary<int, Overlay>();
            _animList = animList;

            bool[] animationArray = new bool[overlayList.Count];
            for (var m = 0; m < animList.Count; m++)
            {
                BGANIM_DEF animation = animList[m];
                for (var n = 0; n < animation.frameList.Count; n++)
                {
                    int index = animation.frameList[n].target;
                    animationArray[index] = true;
                }
            }
            for (int i = 0; i < overlayList.Count; i++)
            {
                BGOVERLAY_DEF info = overlayList[i];
                Layer layer = layers[i];
                bool animationFlag = animationArray[i];
                Overlay overlay = new Overlay(CopyBytesHelper.GetImageData(layer, info, factor), info, i, this.SizeX, this.SizeY, minX, minY, maxX, maxY, animationFlag);
                _overlays[i] = overlay;
            }

            TDMap = new TileDepthMap(_overlays, this.SizeX, this.SizeY);
        }
    }

    public class Padding
    {
        private Overlay _originalOverlay;
        private Tile _originalTile;
        private List<Overlay> _overlayList;
        public PaddingType type;
        public PaddingCoords paddingCoords;
        public PaddingCoords paddingCoordsSelf;

        public Overlay GetOriginalOverlay()
        {
            return _originalOverlay;
        }

        public Tile GetOriginalTile()
        {
            return _originalTile;
        }

        public List<Overlay> GetOverlayList()
        {
            return _overlayList;
        }

        public Padding(Tile originalTile, Overlay originalOverlay, List<Overlay> overlayList, PaddingCoords paddingCoords,
            PaddingCoords paddingCoordsSelf, PaddingType paddingType)
        {
            _originalTile = originalTile;
            _originalOverlay = originalOverlay;
            _overlayList = overlayList;
            this.paddingCoords = paddingCoords;
            this.paddingCoordsSelf = paddingCoordsSelf;
            this.type = paddingType;
        }

        public MemoriaRect GetImageRect()
        {
            return new MemoriaRect();
        }
    }

    public class MemoriaRect
    {

    }

    public enum PaddingType { Down, Left, Up, Right, DownLeft, DownRight, UpLeft, UpRight }

    public class Overlay
    {
        public readonly BGOVERLAY_DEF info;
        public readonly byte[] imageData;
        private readonly Tile[,] _map;
        private readonly int _orderNumber;
        private int _sizeX, _sizeY;
        private bool _animationFlag;
        private readonly bool _isAlpha;

        public int GetOrderNumber()
        {
            return _orderNumber;
        }

        public bool IsAlpha()
        {
            return _isAlpha;
        }

        public bool IsAnimation()
        {
            return _animationFlag;
        }

        public Overlay(byte[] layerImageData, BGOVERLAY_DEF overlayInfo, int orderNumber,
            int sizeX, int sizeY, Int32 minX, Int32 minY, Int32 maxX, Int32 maxY, bool animationFlag)
        {
            info = overlayInfo;
            imageData = layerImageData;
            _orderNumber = orderNumber;
            _map = new Tile[sizeX, sizeY];
            _sizeX = sizeX;
            _sizeY = sizeY;
            _animationFlag = animationFlag;
            _isAlpha = false;
            if (info.spriteList.Count > 0 && info.spriteList[0].alpha != 0)
                _isAlpha = true;
            for (var k = 0; k < info.spriteList.Count; k++)
            {
                Tile tileToPush = new Tile();
                tileToPush.info = info.spriteList[k];
                BGSPRITE_LOC_DEF spriteInfo = info.spriteList[k];
                tileToPush.x = (info.orgX + spriteInfo.offX - minX) / 16; // get global x
                tileToPush.y = (info.orgY + spriteInfo.offY - minY) / 16; // get global y
                tileToPush.overlay = this;
                _map[tileToPush.x, tileToPush.y] = tileToPush;
            }
            // for debug
            //Log.Message($"Overlay {orderNumber} map");
            //for(var j = 0; j < _sizeY; j++)
            //{
            //    String str = "";
            //    for(var i = 0; i < _sizeX; i++)
            //    {
            //        str += _map[i, j].info == null ? "0" : "1";
            //    }
            //    Log.Message(str);
            //}
        }

        public Tile GetTile(int x, int y)
        {
            return _map[x, y];
        }

        public MemoriaRect GetImageRect()
        {
            return new MemoriaRect();
        }

        public Padding[] GetPadding()
        {
            // four paddings
            return new Padding[4];
        }
    }
}
