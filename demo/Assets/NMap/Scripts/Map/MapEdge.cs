using Unity.Mathematics;

namespace ET
{
    public class MapEdge
    {
        public int index;
        public MapCenter d0, d1;  // Delaunay edge
        public MapCorner v0, v1;  // Voronoi edge
        public float2 midpoint;  // halfway between v0,v1
        public int river;  // volume of water, or 0
    }
}
