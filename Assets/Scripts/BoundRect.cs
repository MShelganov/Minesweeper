using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pazzle.game
{
    public class BoundRect
    {
        public float MaxX;
        public float MinX;
        public float MaxZ;
        public float MinZ;

        public BoundRect(float maxX, float minX, float maxZ, float minZ)
        {
            MaxX = maxX;
            MinX = minX;
            MaxZ = maxZ;
            MinZ = minZ;
        }
    }
}
