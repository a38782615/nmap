using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace ET
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DrawMap : MonoBehaviour
    {
        public MapNodeType mapNodeType;
        public MeshRenderer m_meshRenderer;
        public MeshFilter meshFilter;
        public Texture2D mainTexture;
        public Texture2D overlayTexture;
        private MaterialPropertyBlock m_matPropBlock;
        private MapLogic m_mapLogic;
        [SerializeField] private int m_sortingLayer = 0;
        [SerializeField] private int m_orderInLayer = 0;

        public int OrderInLayer
        {
            get { return m_meshRenderer.sortingOrder; }
            set { m_meshRenderer.sortingOrder = value; }
        }

        public int SortingLayerID
        {
            get { return m_meshRenderer.sortingLayerID; }
            set { m_meshRenderer.sortingLayerID = value; }
        }

        public string SortingLayerName
        {
            get { return m_meshRenderer.sortingLayerName; }
            set { m_meshRenderer.sortingLayerName = value; }
        }

        private void CreateMesh()
        {
            meshFilter = GetComponent<MeshFilter>();
            m_meshRenderer = GetComponent<MeshRenderer>();
            if (m_meshRenderer != null)
            {
                SortingLayerID = m_sortingLayer;
                OrderInLayer = m_orderInLayer;
            }

            if (m_mapLogic == null)
            {
                m_mapLogic = new MapLogic();
                m_mapLogic.Init();
            }

            m_mapLogic.Clear();
            meshFilter.sharedMesh = new Mesh
            {
                hideFlags = HideFlags.HideAndDontSave,
                name = name + "_mesh"
            };
            meshFilter.sharedMesh.Clear();
            if (m_matPropBlock == null)
            {
                m_matPropBlock = new MaterialPropertyBlock();
            }

            m_meshRenderer.GetPropertyBlock(m_matPropBlock);
            if (mainTexture != null)
            {
                m_matPropBlock.SetTexture("_MainTex", mainTexture);
            }

            if (overlayTexture != null)
            {
                m_matPropBlock.SetTexture("_Texture2DCover", overlayTexture);
            }
            m_meshRenderer.SetPropertyBlock(m_matPropBlock);
        }

        List<Vector3> s_vertices;
        List<Vector2> m_uv;
        List<Vector2> m_uv2;

        private void Render()
        {
            var mesh = meshFilter.sharedMesh;
            mesh.SetVertices(ToList(m_mapLogic.s_vertices, s_vertices));
            mesh.SetTriangles(m_mapLogic.s_triangles, 0);
            mesh.SetUVs(0, ToList(m_mapLogic.m_uv, m_uv));
            mesh.SetUVs(1, ToList(m_mapLogic.m_uv2, m_uv2));
        }

        public List<Vector2> ToList(List<float2> points, List<Vector2> ps)
        {
            if (ps == null)
            {
                ps = new List<Vector2>(points.Count);
            }

            ps.Clear();
            foreach (var v in points)
            {
                ps.Add((Vector2)v);
            }

            return ps;
        }

        public List<Vector3> ToList(List<float3> points, List<Vector3> ps)
        {
            if (ps == null)
            {
                ps = new List<Vector3>(points.Count);
            }

            ps.Clear();
            foreach (var v in points)
            {
                ps.Add((Vector3)v);
            }

            return ps;
        }

        public float size = 10;
        public uint seed = 10;
        public int NumberOfVertices = 1000;

        private Dictionary<int2, MapNode> GenMap()
        {
            // var n = new MapNode()
            // {
            //     NodeType = mapNodeType,
            //     Pos = new int2(i, j)
            // };
            // map[n.Pos] = n;
            var map = new Dictionary<int2, MapNode>();
            return map;
        }

        public void UpdateNode(int x, int y)
        {
            var pos = new int2(x, y);
            m_mapLogic.Map[pos] = new MapNode()
            {
                NodeType = this.mapNodeType,
                Pos = pos
            };
        }

        private void OnValidate()
        {
            CreateMesh();
            var map = GenMap();
            m_mapLogic.CreateMap(map);
            Render();
        }
    }
}