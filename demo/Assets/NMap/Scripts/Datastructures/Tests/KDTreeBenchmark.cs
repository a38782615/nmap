﻿/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace DataStructures.ViliWonka.Tests
{
    public class KDTreeBenchmark : MonoBehaviour
    {
        float3[] points10k;
        float3[] points100k;
        float3[] points1m;

        float3[] testingArray;
        Stopwatch stopwatch;

        void Awake()
        {
            points10k = new float3[10000];
            points100k = new float3[100000];
            points1m = new float3[1000000];

            stopwatch = new Stopwatch();
        }

        private void Start()
        {
            testingArray = points10k;
            Debug.Log(" -- 10K THOUSAND POINTS --");
            TestSet();

            testingArray = points100k;
            Debug.Log(" -- 100K THOUSAND POINTS --");
            TestSet();

            testingArray = points1m;
            Debug.Log(" -- 1 MILLION POINTS --");
            TestSet();
        }

        void TestSet()
        {
            // Debug.Log(testingArray.Length + " random points for each test:");

            TestConstruction(5, "Uniform", RandomizeUniform);
            TestConstruction(5, "Triangular", RandomizeUniform);
            TestConstruction(5, "2D planar", Randomize2DPlane);
            TestConstruction(5, "2D planar, sorted", SortedRandomize2DPlane);

            TestQuery(5, "Uniform", RandomizeUniform);
            TestQuery(5, "Triangular", RandomizeUniform);
            TestQuery(5, "2D planar", Randomize2DPlane);
            TestQuery(5, "2D planar, sorted", SortedRandomize2DPlane);
        }

        void TestConstruction(int tests, string distributionName, System.Action randomize)
        {
            long sum = 0;
            for (int i = 0; i < tests; i++)
            {
                randomize();
                long time = Construct();

                sum += time;
            }

            Debug.Log("Average " + distributionName + " distribution construction time: " + (long)(sum / (float)tests) +
                      " ms");
        }

        void TestQuery(int tests, string distributionName, System.Action randomize)
        {
            randomize();
            Construct();

            long sum = 0;
            for (int i = 0; i < tests; i++)
            {
                sum += QueryRadius();
            }

            Debug.Log(distributionName + " distribution average query time: " + (long)(sum / (float)tests) + " ms");
        }


        // uniform distribution
        void RandomizeUniform()
        {
            for (int i = 0; i < testingArray.Length; i++)
            {
                testingArray[i] = new float3(
                    Random.value,
                    Random.value,
                    Random.value
                );
            }
        }

        // triangle distribution
        void RandomizeTriangle()
        {
            for (int i = 0; i < testingArray.Length; i++)
            {
                testingArray[i] = new float3(
                    (Random.value + Random.value) / 2f,
                    (Random.value + Random.value) / 2f,
                    (Random.value + Random.value) / 2f
                );
            }
        }

        // 2D plane, with 10% of noise
        void Randomize2DPlane()
        {
            // if U and V are very similar => degenerate plane aka line
            Vector3 U = Random.onUnitSphere;
            Vector3 V = Random.onUnitSphere;

            for (int i = 0; i < testingArray.Length; i++)
                testingArray[i] = Random.value * U + Random.value * V + Random.insideUnitSphere * 0.1f;
        }

        void SortedRandomize2DPlane()
        {
            Randomize2DPlane();

            //Sort by all coordinates
            System.Array.Sort<float3>(testingArray, (v1, v2) => v1.x.CompareTo(v2.x));
            System.Array.Sort<float3>(testingArray, (v1, v2) => v1.y.CompareTo(v2.y));
            System.Array.Sort<float3>(testingArray, (v1, v2) => v1.z.CompareTo(v2.z));
        }

        ET.KDTree tree;

        long Construct()
        {
            stopwatch.Reset();
            stopwatch.Start();

            tree = new ET.KDTree();

            tree.Build(testingArray);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        ET.KDQuery query = new ET.KDQuery();
        List<int> results = new List<int>();

        long QueryRadius()
        {
            stopwatch.Reset();
            stopwatch.Start();

            float3 position = new float3(1, 1, 1) * 0.5f + (float3)Random.insideUnitSphere;
            float radius = 0.25f;

            results.Clear();
            query.Radius(tree, position, radius, results);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        long QueryClosest()
        {
            stopwatch.Reset();
            stopwatch.Start();

            float3 position = new float3(1, 1, 1) * 0.5f + (float3)Random.insideUnitSphere;
            float radius = 0.25f;

            results.Clear();
            query.ClosestPoint(tree, position, results);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        long QueryKNearest()
        {
            stopwatch.Reset();
            stopwatch.Start();

            float3 position = new float3(1, 1, 1) * 0.5f + (float3)Random.insideUnitSphere;
            int k = 13;

            results.Clear();
            query.KNearest(tree, position, k, results);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        long QueryInterval()
        {
            stopwatch.Reset();
            stopwatch.Start();

            float3 randOffset = Random.insideUnitSphere * 0.25f;

            float3 min = new float3(1, 1, 1) * 0.25f + (float3)Random.insideUnitSphere * 0.25f + randOffset;
            float3 max = new float3(1, 1, 1) * 0.75f + (float3)Random.insideUnitSphere * 0.25f + randOffset;

            results.Clear();
            query.Interval(tree, min, max, results);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}