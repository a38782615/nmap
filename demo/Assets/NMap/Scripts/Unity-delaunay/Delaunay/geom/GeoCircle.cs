using Unity.Mathematics;

namespace ET
{
    public sealed class GeoCircle
    {
        public float2 center;
        public float radius;

        public GeoCircle(float centerX, float centerY, float radius)
        {
            this.center = new float2(centerX, centerY);
            this.radius = radius;
        }

        public override string ToString()
        {
            return "Circle (center: " + center.ToString() + "; radius: " + radius.ToString() + ")";
        }
    }
}