using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public static class DrawUtil
{
    public static List<float3> ToList(List<Vector3> points, List<float3> ps)
    {
        ps.Clear();
        foreach (var v in points)
        {
            ps.Add(v);
        }

        return ps;
    }
    public static List<float2> ToList(List<Vector2> points, List<float2> ps)
    {
        ps.Clear();
        foreach (var v in points)
        {
            ps.Add(v);
        }

        return ps;
    }
    public static List<Vector2> ToList(List<float2> points, List<Vector2> ps)
    {
        ps.Clear();
        foreach (var v in points)
        {
            ps.Add(v);
        }

        return ps;
    }
    public static List<Vector3> ToList(List<float3> points, List<Vector3> ps)
    {
        ps.Clear();
        foreach (var v in points)
        {
            ps.Add(v);
        }
        return ps;
    }

    public static void DrawLine(this Texture2D tex, int x0, int y0, int x1, int y1, Color col)
    {
        int dy = (int)(y1 - y0);
        int dx = (int)(x1 - x0);
        int stepx, stepy;

        if (dy < 0)
        {
            dy = -dy;
            stepy = -1;
        }
        else
        {
            stepy = 1;
        }

        if (dx < 0)
        {
            dx = -dx;
            stepx = -1;
        }
        else
        {
            stepx = 1;
        }

        dy <<= 1;
        dx <<= 1;

        float fraction = 0;

        tex.SetPixel(x0, y0, col);
        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (math.abs(x0 - x1) > 1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;
                }

                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, col);
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (math.abs(y0 - y1) > 1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }

                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, col);
            }
        }
    }

    public static void FillPolygon(this Texture2D texture, float2[] points, Color color)
    {
        // http://alienryderflex.com/polygon_fill/

        var IMAGE_BOT = (int)points.Max(p => p.y);
        var IMAGE_TOP = (int)points.Min(p => p.y);
        var IMAGE_LEFT = (int)points.Min(p => p.x);
        var IMAGE_RIGHT = (int)points.Max(p => p.x);
        var MAX_POLY_CORNERS = points.Count();
        var polyCorners = MAX_POLY_CORNERS;
        var polyY = points.Select(p => p.y).ToArray();
        var polyX = points.Select(p => p.x).ToArray();
        int[] nodeX = new int[MAX_POLY_CORNERS];
        int nodes, pixelX, i, j, swap;

        //  Loop through the rows of the image.
        for (int pixelY = IMAGE_TOP; pixelY <= IMAGE_BOT; pixelY++)
        {
            //  Build a list of nodes.
            nodes = 0;
            j = polyCorners - 1;
            for (i = 0; i < polyCorners; i++)
            {
                if (polyY[i] < (float)pixelY && polyY[j] >= (float)pixelY ||
                    polyY[j] < (float)pixelY && polyY[i] >= (float)pixelY)
                {
                    nodeX[nodes++] =
                        (int)(polyX[i] + (pixelY - polyY[i]) / (polyY[j] - polyY[i]) * (polyX[j] - polyX[i]));
                }

                j = i;
            }

            //  Sort the nodes, via a simple “Bubble” sort.
            i = 0;
            while (i < nodes - 1)
            {
                if (nodeX[i] > nodeX[i + 1])
                {
                    swap = nodeX[i];
                    nodeX[i] = nodeX[i + 1];
                    nodeX[i + 1] = swap;
                    if (i != 0) i--;
                }
                else
                {
                    i++;
                }
            }

            //  Fill the pixels between node pairs.
            for (i = 0; i < nodes; i += 2)
            {
                if (nodeX[i] >= IMAGE_RIGHT)
                    break;
                if (nodeX[i + 1] > IMAGE_LEFT)
                {
                    if (nodeX[i] < IMAGE_LEFT)
                        nodeX[i] = IMAGE_LEFT;
                    if (nodeX[i + 1] > IMAGE_RIGHT)
                        nodeX[i + 1] = IMAGE_RIGHT;
                    for (j = nodeX[i]; j < nodeX[i + 1]; j++)
                        texture.SetPixel(j, pixelY, color);
                }
            }
        }
    }
}