using System.Collections.Generic;
using Unity.Mathematics;

namespace ET
{
    public sealed class GeoPolygon
    {
        private List<float2> _vertices;

        public GeoPolygon(List<float2> vertices)
        {
            _vertices = vertices;
        }

        public float Area()
        {
            // XXX: I'm a bit nervous about this; not sure what the * 0.5 is for, bithacking?
            return math.abs(SignedDoubleArea() *
                            0.5f);
        }

        public GeoWinding Winding()
        {
            float signedDoubleArea = SignedDoubleArea();
            if (signedDoubleArea < 0)
            {
                return GeoWinding.CLOCKWISE;
            }

            if (signedDoubleArea > 0)
            {
                return GeoWinding.COUNTERCLOCKWISE;
            }

            return GeoWinding.NONE;
        }

        // XXX: I'm a bit nervous about this because Actionscript represents everything as doubles, not floats
        private float SignedDoubleArea()
        {
            int index, nextIndex;
            int n = _vertices.Count;
            float2 point, next;
            float signedDoubleArea = 0; // Losing lots of precision?
            for (index = 0; index < n; ++index)
            {
                nextIndex = (index + 1) % n;
                point = _vertices[index];
                next = _vertices[nextIndex];
                signedDoubleArea += point.x * next.y - next.x * point.y;
            }

            return signedDoubleArea;
        }
    }
}