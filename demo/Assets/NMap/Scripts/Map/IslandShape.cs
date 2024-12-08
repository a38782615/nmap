using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace ET
{
    public class IslandShape
    {
        // This class has factory functions for generating islands of
        // different shapes. The factory returns a function that takes a
        // normalized point (x and y are -1 to +1) and returns true if the
        // point should be on the island, and false if it should be water
        // (lake or ocean).

        // The radial island radius is based on overlapping sine waves 
        public static float ISLAND_FACTOR = 1.07f;  // 1.0 means no small islands; 2.0 leads to a lot
        public static System.Func<float2, bool> makeRadial()
        {
            var r = Random.CreateFromIndex(1);
            var bumps = r.NextInt(1, 6);
            var startAngle = r.NextInt() * 2 * math.PI;
            var dipAngle = r.NextInt() * 2 * math.PI;

            var random = r.NextInt();
            var start = 0.2f;
            var end = 0.7f;

            var dipWidth = (end - start) * random + start;

            System.Func<float2, bool> inside = q =>
            {
                var angle = math.atan2(q.y, q.x);
                var length = 0.5 * (math.max(math.abs(q.x), math.abs(q.y)) + q.magnitude());

                var r1 = 0.5 + 0.40 * math.sin(startAngle + bumps * angle + math.cos((bumps + 3) * angle));
                var r2 = 0.7 - 0.20 * math.sin(startAngle + bumps * angle - math.sin((bumps + 2) * angle));
                if (math.abs(angle - dipAngle) < dipWidth
                    || math.abs(angle - dipAngle + 2 * math.PI) < dipWidth
                    || math.abs(angle - dipAngle - 2 * math.PI) < dipWidth)
                {
                    r1 = r2 = 0.2;
                }
                var result = (length < r1 || (length > r1 * ISLAND_FACTOR && length < r2));
                return result;
            };

            return inside;
        }

        // The Perlin-based island combines perlin noise with the radius
        public static System.Func<float2, bool> makePerlin()
        {
            var r = Random.CreateFromIndex(1);
            var offset = r.NextInt(0, 100000);
            System.Func<float2, bool> inside = q =>
            {
                var x = q.x + offset;
                var y = q.y + offset;
                var perlin = Perlin.Noise(x/10 , y/10);
                var checkValue = (0.3 + 0.3 * q.magnitude() * q.magnitude());
                var result = perlin > .3;
                return result;
            };
            return inside;
        }

        // The square shape fills the entire space with land
        public static System.Func<float2, bool> makeSquare()
        {
            System.Func<float2, bool> inside = q => { return true; };
            return inside;
        }
    }
}
