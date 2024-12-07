using System.Collections.Generic;
using System.Drawing;
using Assets.Map;
using Unity.Mathematics;

namespace ET
{
    public struct MapNode
    {
        public int2 Pos;
        public Center Center;
    }

    public struct UVTileCover
    {
        public static int TileCount = 7;
        public static int tileSize = 136;
        public static int atlasWidth = 1024;
        public static float cellSize = tileSize * 1f / atlasWidth;
        public int Id;
        public int2 position;
        public RectangleF uvRect;

        public UVTileCover(int2 pos)
        {
            Id = GetId(pos);
            position = pos%TileCount;
            var tx = position.x * cellSize;
            var ty = 1 - (position.y + 1) * cellSize;
            uvRect = new RectangleF(tx, ty, cellSize, cellSize);
        }

        public static int GetId(int2 xy)
        {
            xy %= TileCount;
            return xy.x + xy.y * TileCount;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }

    public struct UVTileMain
    {
        public static int tileSize = 64;
        public static int atlasWidth = 512;
        public static int TileCount = atlasWidth / tileSize;
        public static float cellSize = tileSize * 1f / atlasWidth;
        public int2 position;
        public RectangleF uvRect;
        public int Id;
        
        public UVTileMain(int2 pos)
        {
            position = pos % TileCount;
            Id = GetId(pos);
            var tx = position.x * cellSize;
            var ty = position.y * cellSize;
            uvRect = new RectangleF(tx, ty, cellSize, cellSize);
        }

        public static int GetId(int2 xy)
        {
            xy %= TileCount;
            return xy.x + xy.y * TileCount;
        }
        
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class Brush
    {
        //1 2 4
        //8 0 16
        //32 64 128 
        public static int[] MaskI = new int[] { 1 << 5, 1 << 6, 1 << 7, 1 << 3, 0, 1 << 4, 1, 1 << 1, 1 << 2 };

        public static Dictionary<int, int> MaskDic = new Dictionary<int, int>()
        {
            { 214, 1 }, { 248, 2 }, { 208, 3 }, { 107, 4 }, { 66, 5 }, { 104, 6 }, { 64, 7 },
            { 31, 8 }, { 22, 9 }, { 24, 10 }, { 16, 11 }, { 11, 12 }, { 2, 13 }, { 8, 14 }, { 0, 15 },
            { 255, 16 }, { 254, 17 }, { 251, 18 }, { 250, 19 }, { 127, 20 }, { 126, 21 }, { 123, 22 }, { 122, 23 },
            { 223, 24 }, { 222, 25 }, { 219, 26 }, { 218, 27 }, { 95, 28 }, { 94, 29 }, { 91, 30 }, { 90, 31 },
            { 120, 32 }, { 75, 33 }, { 30, 34 }, { 210, 35 }, { 88, 36 }, { 74, 37 }, { 26, 38 }, { 82, 39 },
            { 216, 40 }, { 106, 41 }, { 27, 42 }, { 86, 43 }, { 80, 44 }, { 72, 45 }, { 18, 46 }, { 10, 47 }
        };
        public Dictionary<int, UVTileCover> m_uvMap = new Dictionary<int, UVTileCover>();
        public Dictionary<int, UVTileMain> m_uv2Map = new Dictionary<int, UVTileMain>();

        public Brush()
        {
            m_uvMap.Clear();
            m_uv2Map.Clear();
            for (int j = 0; j < UVTileCover.TileCount; j++)
            {
                for (int i = 0; i < UVTileCover.TileCount; i++)
                {
                    var uv = new UVTileCover(new int2(i, j));
                    m_uvMap.Add(uv.Id, uv);
                }
            }

            for (int j = 0; j < UVTileMain.TileCount; j++)
            {
                for (int i = 0; i < UVTileMain.TileCount; i++)
                {
                    var uvtile2 = new UVTileMain(new int2(i, j));
                    m_uv2Map.Add(uvtile2.Id, uvtile2);
                }
            }
        }
    }
}