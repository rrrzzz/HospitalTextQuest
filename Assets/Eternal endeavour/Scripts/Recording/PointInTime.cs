using System;
using UnityEngine;

namespace Recording
{
    [Serializable]
    public struct PointInTime
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public PointInTime(Vector3 pos, Quaternion rot)
        {
            Position = pos;
            Rotation = rot;
        }
    }
}
