using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace ET
{
    public class MapTexture
    {
        private readonly int _textureScale;

        public MapTexture(int textureScale)
        {
            _textureScale = textureScale;
        }

        public Texture2D GetTexture(BiomeMap biomeMap, NoisyEdges noisyEdge)
        {
            int textureWidth = (int)BiomeMap.Width * _textureScale;
            int textureHeight = (int)BiomeMap.Height * _textureScale;

            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
            texture.SetPixels(Enumerable.Repeat(BiomeProperties.Colors[Biome.Ocean], textureWidth * textureHeight).ToArray());

            //绘制扰乱的边缘
            foreach (MapCenter p in biomeMap.MapGraph.centers)
            {
                foreach (var r in p.neighbors)
                {
                    MapEdge mapEdge = biomeMap.MapGraph.lookupEdgeFromCenter(p, r);
                    if (!noisyEdge.path0.ContainsKey(mapEdge.index) || !noisyEdge.path1.ContainsKey(mapEdge.index))
                    {
                        // It's at the edge of the map, where we don't have
                        // the noisy edges computed. TODO: figure out how to
                        // fill in these edges from the voronoi library.
                        continue;
                    }
                    //绘制扰乱后的形状
                    DrawNoisyPolygon(texture, p, noisyEdge.path0[mapEdge.index]);
                    DrawNoisyPolygon(texture, p, noisyEdge.path1[mapEdge.index]);
                }
            }
            //绘制扰乱后的河流
            foreach (var line in biomeMap.MapGraph.edges.Where(p => p.river > 0 && !p.d0.water && !p.d1.water))
            {
                //绘制扰乱后的边缘
                List<float2> edge0 = noisyEdge.path0[line.index];
                for (int i = 0; i < edge0.Count - 1; i++)
                {
                    DrawLine(texture, edge0[i].x, edge0[i].y, edge0[i + 1].x, edge0[i + 1].y, Color.blue);
                }

                List<float2> edge1 = noisyEdge.path1[line.index];
                for (int i = 0; i < edge1.Count - 1; i++)
                {
                    DrawLine(texture, edge1[i].x, edge1[i].y, edge1[i + 1].x, edge1[i + 1].y, Color.blue);
                }
            }

            texture.Apply();

            return texture;
        }

        public void AttachTexture(GameObject plane, BiomeMap biomeMap, NoisyEdges noisyEdge)
        {
            Texture2D texture = GetTexture(biomeMap, noisyEdge);
            plane.GetComponent<Renderer>().material.mainTexture = texture;
        }

        readonly List<float2> _edgePoints = new List<float2>();
        private void DrawNoisyPolygon(Texture2D texture, MapCenter p, List<float2> orgEdges)
        {
            _edgePoints.Clear();
            _edgePoints.AddRange(orgEdges);
            _edgePoints.Add(p.point);
            texture.FillPolygon(
                _edgePoints.Select(x => new float2(x.x * _textureScale, x.y * _textureScale)).ToArray(),
                BiomeProperties.Colors[p.biome]);
        }

        private void DrawLine(Texture2D texture, float x0, float y0, float x1, float y1, Color color)
        {
            texture.DrawLine((int) (x0*_textureScale), (int) (y0*_textureScale), (int) (x1*_textureScale),
                (int) (y1*_textureScale), color);
        }
    }
}