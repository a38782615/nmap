/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using Unity.Mathematics;

namespace ET
{
    public partial struct KDBounds
    {
        public KDBounds(float3 min, float3 max)
        {
            this.min = min;
            this.max = max;
            m_Extents = (max - min) * 0.5f;
            m_Center = min + m_Extents;
        }
        
        public float3 min;
        public float3 max;

        public float3 size
        {
            get { return max - min; }
        }

        public float3 ClosestPoint(float3 point)
        {
            // X axis
            if (point.x < min.x) point.x = min.x;
            else if (point.x > max.x) point.x = max.x;

            // Y axis
            if (point.y < min.y) point.y = min.y;
            else if (point.y > max.y) point.y = max.y;

            // Z axis
            if (point.z < min.z) point.z = min.z;
            else if (point.z > max.z) point.z = max.z;

            return point;
        }

        private float3 m_Center;

        public float3 center
        {
            get => this.m_Center;
            set => this.m_Center = value;
        }

        private float3 m_Extents;

        public float3 extents
        {
            get => this.m_Extents;
            set => this.m_Extents = value;
        }
 
    }
}