using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public Vector3Serializable playerPosition;
    public float playerYEulerAngle;
    public HashSet<Vector2IntSerializable> felledTreesPositions;
    public int days;
    public float dayCycle;
    public Dictionary<ObjectType, int> objects;
    public List<BuildingObjectInfoSerializable> buildingObjects;
}
