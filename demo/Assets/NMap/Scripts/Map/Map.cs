﻿using Delaunay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Assets.Map
{
    public class Map
    {
        private int _pointCount = 500;
        float _lakeThreshold = 0.3f;
        public static float Width = 50;
        public static float Height = 50;
        const int NUM_LLOYD_RELAXATIONS = 2;

        public Graph Graph { get; private set; }
        public Center SelectedCenter { get; private set; }
        List<uint> colors = new List<uint>();

        public Map()
        {
        }

        public void SetPointNum(int num)
        {
            _pointCount = num;
        }

        private Random random;

        List<float2> points = new List<float2>();
        public void SetSeed(uint seed)
        {
            random = Random.CreateFromIndex(seed);
        }

        public void Init(uint seed, Func<float2, bool> checkIsland = null)
        {
            points.Clear();
            colors.Clear();
            SetSeed(seed);

            for (int i = 0; i < _pointCount; i++)
            {
                colors.Add(0);
                points.Add(new float2(
                    random.NextFloat(0, Width),
                    random.NextFloat(0, Height))
                );
            }

            for (int i = 0; i < NUM_LLOYD_RELAXATIONS; i++)
            {
                points = Graph.RelaxPoints(points, Width, Height).ToList();
            }

            var voronoi = new Voronoi(points, colors, new RectangleF(0, 0, Width, Height));

            checkIsland = checkIsland ?? IslandShape.makePerlin();
            Graph = new Graph(checkIsland, points, voronoi, (int)Width, (int)Height, _lakeThreshold);
        }
    }
}