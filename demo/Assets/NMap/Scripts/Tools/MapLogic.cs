using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;

namespace ET
{ 

    public class MapLogic
    {
        public List<float3> s_vertices;

        public List<float2>
            m_uv; //NOTE: this is the only one not static because it's needed to update the animated tiles

        public List<float2>
            m_uv2; //NOTE: this is the only one not static because it's needed to update the animated tiles

        public List<int> s_triangles;
        //根据数据获取图块
        //在维诺图上画地图的图块

        float2[] s_tileUV = new float2[4];
        private Brush brush;

        public Dictionary<int2, MapNode> Map;

        public MapLogic()
        {
            Init();
        }
        public void Init()
        {
            Map = new Dictionary<int2, MapNode>();
            s_vertices = new List<float3>();
            s_triangles = new List<int>();
            m_uv = new List<float2>();
            m_uv2 = new List<float2>();
            brush = new Brush();
        }

        public void Clear()
        {
            s_vertices.Clear();
            s_triangles.Clear();
            m_uv.Clear();
            m_uv2.Clear();
        }

        void DrawOne(RectangleF posRect, RectangleF tileUV0, RectangleF tileUV1)
        {
            //顶点位置
            float px0 = posRect.Left;
            float py0 = posRect.Bottom;
            float px1 = posRect.Right;
            float py1 = posRect.Top;

            int vertexIdx = s_vertices.Count;
            s_vertices.Add(new float3(px0, py0, 0));
            s_vertices.Add(new float3(px1, py0, 0));
            s_vertices.Add(new float3(px0, py1, 0));
            s_vertices.Add(new float3(px1, py1, 0));
            //三角形
            s_triangles.Add(vertexIdx + 3);
            s_triangles.Add(vertexIdx + 0);
            s_triangles.Add(vertexIdx + 2);
            s_triangles.Add(vertexIdx + 0);
            s_triangles.Add(vertexIdx + 3);
            s_triangles.Add(vertexIdx + 1);
            //UV贴图坐标
            float u00 = tileUV0.Left;
            float v00 = tileUV0.Bottom;
            float u01 = tileUV0.Right;
            float v01 = tileUV0.Top;
            s_tileUV[0] = new float2(u00, v00);
            s_tileUV[1] = new float2(u01, v00);
            s_tileUV[2] = new float2(u00, v01);
            s_tileUV[3] = new float2(u01, v01);
            for (int i = 0; i < 4; ++i)
            {
                m_uv.Add(s_tileUV[i]);
            }

            float u10 = tileUV1.Left;
            float v10 = tileUV1.Bottom;
            float u11 = tileUV1.Right;
            float v11 = tileUV1.Top;
            s_tileUV[0] = new float2(u10, v10);
            s_tileUV[1] = new float2(u11, v10);
            s_tileUV[2] = new float2(u10, v11);
            s_tileUV[3] = new float2(u11, v11);

            for (int i = 0; i < 4; ++i)
            {
                m_uv2.Add(s_tileUV[i]);
            }
        }

        public void CreateMap()
        {
            foreach (var eitem in Map)
            {
                var item = eitem.Value;
                var i = item.Pos.x;
                var j = item.Pos.y;
                var hasNode = HasNode(i, j);
                if (hasNode)
                {
                    var mask = GetMaskFromMap(i, j);
                    var id1 = Brush.MaskDic[mask];
                    var id2 = UVTileMain.GetId(new int2(i, j));
                    DrawOne(
                        new RectangleF(i * UVTileCover.cellSize, j * UVTileCover.cellSize, UVTileCover.cellSize, UVTileCover.cellSize),
                        brush.m_uv2Map[id2].uvRect,
                        brush.m_uvMap[id1].uvRect);
                }
            }
        }

        private bool HasNode(int x, int y)
        { 
            return Map.TryGetValue(new int2(x, y), out var ret);
        }

        private int GetMaskFromMap(int x, int y)
        {
            var mask = 0;
            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    //坐标
                    var xx = x + i;
                    var yy = y + j;

                    //mask坐标
                    var xi = i + 1;
                    var yi = j + 1;
                    var mskid = xi + yi * 3;
                    int msk = 0;
                    if (i == 0 || j == 0)
                    {
                        // 正方向
                        msk = (HasNode(xx, yy) ? Brush.MaskI[mskid] : 0);
                    }
                    else
                    {
                        //角落如果有 需要判断 角落2边HasNode
                        msk = ((HasNode(xx, yy) && HasNode(xx, y) && HasNode(x, yy)) ? Brush.MaskI[mskid] : 0);
                    }

                    mask += msk;
                }
            }

            return mask;
        }
    }
}