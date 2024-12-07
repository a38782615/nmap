using System.Collections.Generic;
using Assets.Map;
using Unity.Mathematics;
using UnityEngine;

namespace ET
{
    public partial class DrawMap
    {
        public GameObject View;
        DrawCarpet ground;
        DrawCarpet grass;

        public DrawMap(GameObject go)
        {
            View = go;
        }

        public void Init()
        {
            ground = new DrawCarpet(View.transform.GetChild(0).gameObject);
            ground.Init(0);
            grass = new DrawCarpet(View.transform.GetChild(1).gameObject);
            grass.Init(1);
        }

        KDTree kdTree;
        KDQuery query;
        private List<float3> centerIdxs = new List<float3>();
        private Dictionary<int2, MapNode> m_map = new Dictionary<int2, MapNode>();
        private List<int> m_queryResult = new List<int>();

        public void GenMap(Map m)
        {
            m_map.Clear();
            ground.Clear();
            grass.Clear();

            centerIdxs.Clear();
            kdTree = new KDTree();
            query = new KDQuery();
            var centers = m.Graph.centers;
            foreach (var c in centers)
            {
                centerIdxs.Add(new float3(c.point, 0));
            }

            kdTree.Build(centerIdxs.ToArray());
            for (int i = 0; i < m.Graph.Width; i++)
            {
                for (int j = 0; j < m.Graph.Height; j++)
                {
                    var p = new float3(i, j, 0);
                    m_queryResult.Clear();
                    query.KNearest(kdTree, p, 1, m_queryResult);
                    if (m_queryResult.Count > 0)
                    {
                        var n = m_queryResult[0];
                        var center = centers[n];
                        var pos = new int2(i, j);
                        m_map[pos] = new MapNode()
                        {
                            Center = center,
                            Pos = pos
                        };
                        if (center.moisture > 0.5f)
                        {
                            grass.Set(pos, m_map[pos]);
                        }
                        else
                        {
                            ground.Set(pos, m_map[pos]);
                        }
                    }
                }
            }
            
            grass.GenMap();
            ground.GenMap();
        }
    }
}