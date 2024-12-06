using System;
using Unity.Mathematics;

namespace Delaunay
{
	namespace Geo
	{
		public sealed class LineSegment
		{
			public static int CompareLengths_MAX (LineSegment segment0, LineSegment segment1)
			{
				float length0 = math.distance((float2)segment0.p0, (float2)segment0.p1);
				float length1 = math.distance ((float2)segment1.p0, (float2)segment1.p1);
				if (length0 < length1) {
					return 1;
				}
				if (length0 > length1) {
					return -1;
				}
				return 0;
			}
		
			public static int CompareLengths (LineSegment edge0, LineSegment edge1)
			{
				return - CompareLengths_MAX (edge0, edge1);
			}

			public Nullable<float2> p0;
			public Nullable<float2> p1;
		
			public LineSegment (Nullable<float2> p0, Nullable<float2> p1)
			{
				this.p0 = p0;
				this.p1 = p1;
			}
		
		}
	}
}