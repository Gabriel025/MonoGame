using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame
{
    struct AABB
    {
        public Vector3 center, size;

        public AABB(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            center = new Vector3(x1 + x2, y1 + y2, z1 + z2);
            center *= 0.5f;
            size = new Vector3(Math.Abs(x1 - x2), Math.Abs(y1 - y2), Math.Abs(z1 - z2));
        }

        public AABB(Vector3 center, Vector3 size)
        {
            this.center = center;
            this.size = size;
        }
    }
}
