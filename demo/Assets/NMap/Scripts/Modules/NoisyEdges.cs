// Annotate each edge with a noisy path, to make maps look more interesting.
// Author: amitp@cs.stanford.edu
// License: MIT

using System.Collections.Generic;
using Assets.Map;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class NoisyEdges
{
    private static readonly float NOISY_LINE_TRADEOFF = 0.5f;// low: jagged vedge; high: jagged dedge
    public Dictionary<int, List<float2>> path0 = new Dictionary<int, List<float2>>();// edge index -> Vector.<Point>
    public Dictionary<int, List<float2>> path1 = new Dictionary<int, List<float2>>();// edge index -> Vector.<Point>

    private const float SizeScale = 0.1f;
    // Build noisy line paths for each of the Voronoi edges. There are
    // two noisy line paths for each edge, each covering half the
    // distance: path0 is from v0 to the midpoint and path1 is from v1
    // to the midpoint. When drawing the polygons, one or the other
    // must be drawn in reverse order.
    public void BuildNoisyEdges(Map map)
    {
        foreach (Center p in map.Graph.centers)
        {
            foreach (Edge edge in p.borders)
            {
                if (edge.d0 != null && edge.d1 != null && edge.v0 != null && edge.v1 != null
                    && !path0.ContainsKey(edge.index))
                {
                    float f = NOISY_LINE_TRADEOFF;
                    float2 t = mathExtensions.Interpolate(edge.v0.point, edge.d0.point, f);
                    float2 q = mathExtensions.Interpolate(edge.v0.point, edge.d1.point, f);
                    float2 r = mathExtensions.Interpolate(edge.v1.point, edge.d0.point, f);
                    float2 s = mathExtensions.Interpolate(edge.v1.point, edge.d1.point, f);

                    float minLength = 10 * SizeScale;
                    if (edge.d0.biome != edge.d1.biome)
                    {
                        minLength = 3 * SizeScale;
                    }

                    if (edge.d0.ocean && edge.d1.ocean)
                    {
                        minLength = 100 * SizeScale;
                    }

                    if (edge.d0.coast || edge.d1.coast)
                    {
                        minLength = 1 * SizeScale;
                    }

                    if (edge.river > 0)
                    {
                        minLength = 1 * SizeScale;
                    }

                    path0[edge.index] = buildNoisyLineSegments(edge.v0.point, t, edge.midpoint, q, minLength);
                    path1[edge.index] = buildNoisyLineSegments(edge.v1.point, s, edge.midpoint, r, minLength);
                }
            }
        }
    }

    // Helper function: build a single noisy line in a quadrilateral A-B-C-D,
    // and store the output points in a Vector.
    private List<float2> buildNoisyLineSegments(float2 A, float2 B, float2 C, float2 D, float minLength)
    {
        List<float2> points = new List<float2>();
        
        points.Add(A);
        subdivide(A, B, C, D,points,minLength);
        points.Add(C);

        return points;
    }

    private void subdivide(float2 A, float2 B, float2 C, float2 D, List<float2> points, float minLength)
    {
        if (math.distance(A,C) < minLength || math.distance(B,D)<minLength)
            return;

        // Subdivide the quadrilateral
        float p = Random.Range(0.2f, 0.8f);// vertical (along A-D and B-C)
        float q = Random.Range(0.2f, 0.8f);// horizontal (along A-B and D-C)

        // Midpoints
        float2 E = mathExtensions.Interpolate(A, D, p);
        float2 F = mathExtensions.Interpolate(B, C, p);
        float2 G = mathExtensions.Interpolate(A, B, q);
        float2 I = mathExtensions.Interpolate(D, C, q);

        // Central point
        float2 H = mathExtensions.Interpolate(E, F, q);

        // Divide the quad into subquads, but meet at H
        float s = 1 - Random.Range(-0.4f, 0.4f);
        float t = 1 - Random.Range(-0.4f, 0.4f);

        subdivide(A, mathExtensions.Interpolate(G, B, s), H, mathExtensions.Interpolate(E, D, t), points,
            minLength);
        points.Add(H);
        subdivide(H, mathExtensions.Interpolate(F, C, s), C, mathExtensions.Interpolate(I, D, t), points,
            minLength);
    }
}
