using Delaunay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Map
{
    public class Map2
    {
        private int _pointCount = 500;
        float _lakeThreshold = 0.3f;
        public static float Width = 50;
        public static float Height = 50;
        const int NUM_LLOYD_RELAXATIONS = 2;

        public Graph Graph { get; private set; }
        public Center SelectedCenter { get; private set; }

        public Map2()
        {
        }
        List<float2> f2l = new List<float2>();
        public void Init(Func<Vector2, bool> checkIsland = null)
        {
            List<uint> colors = new List<uint>();
            var points = new List<Vector2>();

            for (int i = 0; i < _pointCount; i++)
            {
                colors.Add(0);
                points.Add(new Vector2(
                        UnityEngine.Random.Range(0, Width),
                        UnityEngine.Random.Range(0, Height))
                );
            }

            for (int i = 0; i < NUM_LLOYD_RELAXATIONS; i++)
                points = Graph.RelaxPoints(points, Width, Height).ToList();

            f2l.Clear();
            foreach (var p in points)
            {
                f2l.Add(p);
            }
            var voronoi = new Voronoi(f2l, colors, new RectangleF(0, 0, Width, Height));

            checkIsland = checkIsland ?? IslandShape.makePerlin();
            Graph = new Graph(checkIsland, points, voronoi, (int)Width, (int)Height, _lakeThreshold);
        }
    }
}
