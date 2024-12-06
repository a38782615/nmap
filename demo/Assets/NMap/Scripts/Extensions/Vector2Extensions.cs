using System.Linq;
using Unity.Mathematics;

public static class float2Extensions
{
    public static float magnitude(this float2 self)
    {
        return (float) math.sqrt((double) self.x * (double) self.x + (double) self.y * (double) self.y);
    }
    
    public static float2 Set(this  float2 self, float x, float y)
    {
        self.x = x;
        self.y = y;
        return self;
    }
    
    public static float2 Interpolate(float2 pt1, float2 pt2, float f)
    {
        var x = f * pt1.x + (1 - f) * pt2.x;
        var y = f * pt1.y + (1 - f) * pt2.y;

        return new float2(x, y);
    }

    public struct Point
    {
        public short x;
        public short y;
        public Point(short aX, short aY) { x = aX; y = aY; }
        public Point(int aX, int aY) : this((short)aX, (short)aY) { }
    }

}


