using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Vector3Serializable playerPosition;
    public float playerYEulerAngle;
    public HashSet<Vector2IntSerializable> felledTreesPositions;
}
