using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ET
{
    public class MapTexture2
    {
        private readonly int _textureScale;

        public MapTexture2(int textureScale)
        {
            _textureScale = textureScale;
        }

        public void AttachTexture(GameObject plane, Map2 map)
        {
            var textureWidth = (int) Map2.Width*_textureScale;
            var textureHeight = (int) Map2.Height*_textureScale;

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
            texture.SetPixels(Enumerable.Repeat(BiomeProperties.Colors[Biome.Ocean], textureWidth * textureHeight).ToArray());

            //绘制陆地
            var oceanConors = map.MapGraph.centers.Where(p => !p.water).Select(p => p.corners);
            foreach (var conors in oceanConors)
                texture.FillPolygon(
                    conors.Select(p=>p.point * _textureScale).ToArray(),
                    BiomeProperties.Colors[Biome.Beach]);
            //绘制湖泊
            var lakeConors = map.MapGraph.centers.Where(p => p.water && !p.ocean).Select(p => p.corners);
            foreach (var conors in lakeConors)
                texture.FillPolygon(
                    conors.Select(p => p.point * _textureScale).ToArray(),
                    BiomeProperties.Colors[Biome.Lake]);

            //绘制边缘
            var lines = map.MapGraph.edges.Where(p => p.v0 != null).Select(p => new[]
            {
                p.v0.point.x, p.v0.point.y,
                p.v1.point.x, p.v1.point.y
            }).ToArray();

            foreach (var line in lines)
                DrawLine(texture, line[0], line[1], line[2], line[3], Color.black);
            //绘制中心点
            var points = map.MapGraph.centers.Select(p => p.point).ToList();
            foreach (var p in points)
                texture.SetPixel((int) (p.x*_textureScale), (int) (p.y*_textureScale), Color.red);

            texture.Apply();

            plane.GetComponent<Renderer>().material.mainTexture = texture;
        }
        public void ShowElevation(GameObject plane, Map2 map)
        {
            var textureWidth = (int)Map1.Width * _textureScale;
            var textureHeight = (int)Map1.Height * _textureScale;

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
            texture.SetPixels(Enumerable.Repeat(BiomeProperties.Colors[Biome.Ocean], textureWidth * textureHeight).ToArray());

            //绘制陆地
            var lands = map.MapGraph.centers.Where(p => !p.ocean);
            foreach (var land in lands)
                texture.FillPolygon(
                    land.corners.Select(p => p.point * _textureScale).ToArray(),
					BiomeProperties.Colors[Biome.Beach] * land.elevation);

            //绘制边缘
            var lines = map.MapGraph.edges.Where(p => p.v0 != null).Select(p => new[]
            {
                p.v0.point.x, p.v0.point.y,
                p.v1.point.x, p.v1.point.y
            }).ToArray();

            foreach (var line in lines)
                DrawLine(texture, line[0], line[1], line[2], line[3], Color.black);
            //绘制中心点
            var points = map.MapGraph.centers.Select(p => p.point).ToList();
            foreach (var p in points)
                texture.SetPixel((int)(p.x * _textureScale), (int)(p.y * _textureScale), Color.red);

            texture.Apply();

            plane.GetComponent<Renderer>().material.mainTexture = texture;
        }

		public void ShowRivers(GameObject plane, Map2 map)
		{
			
			var textureWidth = (int)Map1.Width * _textureScale;
			var textureHeight = (int)Map1.Height * _textureScale;
			
			var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
			texture.SetPixels(Enumerable.Repeat(BiomeProperties.Colors[Biome.Ocean], textureWidth * textureHeight).ToArray());
			
			//绘制陆地
			var lands = map.MapGraph.centers.Where(p => !p.ocean);
			foreach (var land in lands)
				texture.FillPolygon(
					land.corners.Select(p => p.point * _textureScale).ToArray(),
					BiomeProperties.Colors[Biome.Beach] * land.elevation);
			
			//绘制边缘
			var lines = map.MapGraph.edges.Where(p => p.v0 != null).Select(p => new[]
			                                                            {
				p.v0.point.x, p.v0.point.y,
				p.v1.point.x, p.v1.point.y
			}).ToArray();
			
			foreach (var line in lines)
				DrawLine(texture, line[0], line[1], line[2], line[3], Color.black);
			//绘制中心点
			var points = map.MapGraph.centers.Select(p => p.point).ToList();
			foreach (var p in points)
				texture.SetPixel((int)(p.x * _textureScale), (int)(p.y * _textureScale), Color.red);
		    //绘制河流
			foreach (var line in map.MapGraph.edges.Where(p => p.river > 0 && !p.d0.water && !p.d1.water))
                DrawLine(texture, line.v0.point.x, line.v0.point.y, line.v1.point.x, line.v1.point.y, Color.blue, 10);

			texture.Apply();
			
			plane.GetComponent<Renderer>().material.mainTexture = texture;
		}

		public void DrawMoisture(GameObject plane, Map2 map)
		{
			var textureWidth = (int) Map1.Width*_textureScale;
			var textureHeight = (int) Map1.Height*_textureScale;
			
			var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
			texture.SetPixels(Enumerable.Repeat(BiomeProperties.Colors[Biome.Ocean], textureWidth * textureHeight).ToArray());
			
			//绘制陆地
			var lands = map.MapGraph.centers.Where(p => !p.ocean);
			foreach (var land in lands)
				texture.FillPolygon(
					land.corners.Select(p => p.point * _textureScale).ToArray(),
					BiomeProperties.Colors[Biome.Grassland] * land.moisture);
			//绘制湖泊
			var lakeConors = map.MapGraph.centers.Where(p => p.water && !p.ocean).Select(p => p.corners);
			foreach (var conors in lakeConors)
				texture.FillPolygon(
					conors.Select(p => p.point * _textureScale).ToArray(),
					BiomeProperties.Colors[Biome.Lake]);
			
			//绘制边缘
			var lines = map.MapGraph.edges.Where(p => p.v0 != null).Select(p => new[]
			                                                            {
				p.v0.point.x, p.v0.point.y,
				p.v1.point.x, p.v1.point.y
			}).ToArray();
			
			foreach (var line in lines)
				DrawLine(texture, line[0], line[1], line[2], line[3], Color.black);
			//绘制中心点
			var points = map.MapGraph.centers.Select(p => p.point).ToList();
			foreach (var p in points)
				texture.SetPixel((int) (p.x*_textureScale), (int) (p.y*_textureScale), Color.red);
			
			texture.Apply();
			
			plane.GetComponent<Renderer>().material.mainTexture = texture;
		}

        public void DrawBiome(GameObject plane, Map2 map)
        {
            var textureWidth = (int)Map1.Width * _textureScale;
            var textureHeight = (int)Map1.Height * _textureScale;

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
            texture.SetPixels(Enumerable.Repeat(BiomeProperties.Colors[Biome.Ocean], textureWidth * textureHeight).ToArray());

            //绘制陆地
            var lands = map.MapGraph.centers.Where(p => !p.ocean);
            foreach (var land in lands)
                texture.FillPolygon(
                    land.corners.Select(p => p.point * _textureScale).ToArray(),
                    BiomeProperties.Colors[land.biome]);
            //绘制湖泊
            var lakeConors = map.MapGraph.centers.Where(p => p.water && !p.ocean).Select(p => p.corners);
            foreach (var conors in lakeConors)
                texture.FillPolygon(
                    conors.Select(p => p.point * _textureScale).ToArray(),
                    BiomeProperties.Colors[Biome.Lake]);

            //绘制边缘
            var lines = map.MapGraph.edges.Where(p => p.v0 != null).Select(p => new[]
			                                                            {
				p.v0.point.x, p.v0.point.y,
				p.v1.point.x, p.v1.point.y
			}).ToArray();

            foreach (var line in lines)
                DrawLine(texture, line[0], line[1], line[2], line[3], Color.black);
            //绘制中心点
            var points = map.MapGraph.centers.Select(p => p.point).ToList();
            foreach (var p in points)
                texture.SetPixel((int)(p.x * _textureScale), (int)(p.y * _textureScale), Color.red);
            //绘制河流
            foreach (var line in map.MapGraph.edges.Where(p => p.river > 0 && !p.d0.water && !p.d1.water))
                DrawLine(texture, line.v0.point.x, line.v0.point.y, line.v1.point.x, line.v1.point.y, Color.blue,10);

            texture.Apply();

            plane.GetComponent<Renderer>().material.mainTexture = texture;
        }

        private void DrawLine(Texture2D texture, float x0, float y0, float x1, float y1, Color color,int width =1)
        {
            for (int i = 0; i < width; i++)
            {
                float delta = 0.005f * i;
                texture.DrawLine((int)((x0 + delta) * _textureScale), (int)((y0 + delta) * _textureScale), (int)((x1 + delta) * _textureScale),
                    (int)((y1 + delta) * _textureScale), color);
            }
        }
    }
}