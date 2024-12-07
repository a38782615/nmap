using System.Linq;
using Unity.Mathematics;

public static class mathExtensions
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
    public static float3 InUnitSphere(ref Unity.Mathematics.Random random)
    {
        // 随机方向：球坐标系中的 theta 和 phi
        float theta = random.NextFloat(0f, 2f * math.PI); // 水平方向角度
        float phi = math.acos(random.NextFloat(-1f, 1f)); // 垂直方向角度

        // 随机半径，使用立方根分布确保均匀分布
        float r = math.pow(random.NextFloat(), 1f / 3f);

        // 将球坐标转换为笛卡尔坐标
        float sinPhi = math.sin(phi);
        return new float3(
            r * sinPhi * math.cos(theta),
            r * sinPhi * math.sin(theta),
            r * math.cos(phi)
        );
    }
}


