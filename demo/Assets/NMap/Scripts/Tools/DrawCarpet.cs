using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ET
{
    public partial class DrawCarpet
    {
        public string[] mainNames = { "noise_rocky", "Ground_noise_water_shallow","forest_ground_noise" };
        public string[] overNames = { "blocky","water", "grass" };
        
        public MeshRenderer m_meshRenderer;
        public MeshFilter meshFilter;
        public Texture2D mainTexture;
        public Texture2D overlayTexture;
        private MaterialPropertyBlock m_matPropBlock;
        private MapLogic m_mapLogic;
        public GameObject View;
        List<Vector3> s_vertices;
        List<Vector2> m_uv;
        List<Vector2> m_uv2;

        public DrawCarpet(GameObject go)
        {
            View = go;
            s_vertices = new List<Vector3>();
            m_uv = new List<Vector2>();
            m_uv2 = new List<Vector2>();
        }

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

        public int CarType;
        public void Init(int type)
        {
            CarType = type;
            mainTexture = Resources.Load<Texture2D>("Sprites/"+mainNames[type]);
            overlayTexture = Resources.Load<Texture2D>("Sprites/"+overNames[type]);
            meshFilter = View.GetComponent<MeshFilter>();
            m_meshRenderer = View.GetComponent<MeshRenderer>();
            if (m_meshRenderer != null)
            {
                SortingLayerID = 0;
                OrderInLayer = type;
            }

            if (m_mapLogic == null)
            {
                m_mapLogic = new MapLogic();
            }

            m_mapLogic.Clear();
            meshFilter.sharedMesh = new Mesh
            {
                hideFlags = HideFlags.HideAndDontSave,
                name = View.name + "_mesh"
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


        public void GenMap()
        {
            m_mapLogic.CreateMap();
            Render();
        }

        private void Render()
        {
            var mesh = meshFilter.sharedMesh;
            mesh.SetVertices(DrawUtil.ToList(m_mapLogic.s_vertices, s_vertices));
            mesh.SetTriangles(m_mapLogic.s_triangles, 0);
            mesh.SetUVs(0, DrawUtil.ToList(m_mapLogic.m_uv, m_uv));
            mesh.SetUVs(1, DrawUtil.ToList(m_mapLogic.m_uv2, m_uv2));
        }

        public void UpdateNode(int x, int y)
        {
            var pos = new int2(x, y);
        }

        public void Clear()
        {
            m_mapLogic.Map.Clear();
        }

        public void Set(Func<DrawCarpet,MapNode,bool> func, MapNode node)
        {
            if (func.Invoke(this,node))
            {
                m_mapLogic.Map[node.Pos] = node;
            }
        }
    }
}