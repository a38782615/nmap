using System.Collections.Generic;
using ET;
using Unity.Mathematics;
using UnityEngine;

namespace ET
{
    public partial class DrawMap
    {
        public GameObject View;
        private List<DrawCarpet> grounds;

        public DrawMap(GameObject go)
        {
            View = go;
        }

        public void Init()
        {
            var c = View.transform.childCount;
            grounds = new List<DrawCarpet>(c);
            for (int i = 0; i < c; i++)
            {
                var g  = new DrawCarpet(View.transform.GetChild(i).gameObject);
                grounds.Add(g);
                g.Init(i);
            }
        }

        KDTree kdTree;
        KDQuery query;
        private List<float3> centerIdxs = new List<float3>();
        private Dictionary<int2, MapNode> m_map = new Dictionary<int2, MapNode>();
        private List<int> m_queryResult = new List<int>();

        public void GenMap(BiomeMap m)
        {
            var self = this;
            grounds.ForEach((e) =>
            {
                e.Clear();
            });
            kdTree = new KDTree();
            query = new KDQuery();
            var centers = m.MapGraph.centers;
            foreach (var c in centers)
            {
                centerIdxs.Add(new float3(c.point, 0));
            }

            kdTree.Build(centerIdxs.ToArray());
            for (int i = 0; i < m.MapGraph.Width; i++)
            {
                for (int j = 0; j < m.MapGraph.Height; j++)
                {
                    var p = new float3(i, j, 0);
                    m_queryResult.Clear();
                    query.KNearest(kdTree, p, 1, m_queryResult);
                    if (m_queryResult.Count > 0)
                    {
                        var n = m_queryResult[0];
                        var center = centers[n];
                        var pos = new int2(i, j);
                        var node = new MapNode()
                        {
                            MapCenter = center,
                            Pos = pos
                        };
                        m_map[pos] = node;
                        grounds.ForEach((e) =>
                        {
                            e.Set(self.IsGround, node);
                        });
                    }
                }
            }
            
            grounds.ForEach((e) =>
            {
                e.GenMap();
            });
        }

        public bool IsGround(DrawCarpet carpet,MapNode node)
        {
            bool b = false;
            if (carpet.CarType == 0)
            {
                b = true;
            }
            else if (carpet.CarType == 1)
            {
                b = IsWater(node);
            }
            else if (carpet.CarType == 2)
            {
                b = IsGrass(node);
            }

            return b;
        }

        public bool IsWater(MapNode node)
        {
            var b = node.MapCenter.biome == Biome.Ocean || node.MapCenter.biome == Biome.Lake || node.MapCenter.biome == Biome.TropicalRainForest || node.MapCenter.biome == Biome.Ice;
            return b;
        }

        public bool IsGrass(MapNode node)
        {
            var b = node.MapCenter.biome == Biome.Grassland;
            return b;
        }
    }
}