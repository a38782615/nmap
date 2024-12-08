using System.Collections.Generic;
using Unity.Mathematics;

namespace ET
{
    public class MapCorner
    {
        public int index;

        public float2 point;  // location
        public bool ocean;  // ocean
        public bool water;  // lake or ocean
        public bool coast;  // touches ocean and land polygons
        public bool border;  // at the edge of the map
        public float elevation;  // 0.0-1.0
        public float moisture;  // 0.0-1.0

        public List<MapCenter> touches = new List<MapCenter>();
        public List<MapEdge> protrudes = new List<MapEdge>();
        public List<MapCorner> adjacent = new List<MapCorner>();

        public int river;  // 0 if no river, or volume of water in river
        public MapCorner downslope;  // pointer to adjacent corner most downhill
        public MapCorner watershed;  // pointer to coastal corner, or null
        public int watershed_size;
    }
}
