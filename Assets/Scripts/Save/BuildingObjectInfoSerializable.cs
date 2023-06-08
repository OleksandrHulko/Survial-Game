using System;

[Serializable]
public class BuildingObjectInfoSerializable
{
    public ObjectType objectType;
    public Vector2IntSerializable position;
    public Vector3IntSerializable eulerAngles;
    public int height;

    BuildingObjectInfoSerializable(BuildingObjectInfo buildingObjectInfo)
    {
        objectType  = buildingObjectInfo.objectType;
        position    = buildingObjectInfo.position;
        eulerAngles = buildingObjectInfo.eulerAngles;
        height      = buildingObjectInfo.height;
    }
    
    public static implicit operator BuildingObjectInfoSerializable(BuildingObjectInfo buildingObjectInfo)
    {
        return new BuildingObjectInfoSerializable(buildingObjectInfo);
    }

    public static implicit operator BuildingObjectInfo(BuildingObjectInfoSerializable buildingObjectInfoSerializable)
    {
        return new BuildingObjectInfo()
        {
              objectType  = buildingObjectInfoSerializable.objectType
            , position    = buildingObjectInfoSerializable.position
            , eulerAngles = buildingObjectInfoSerializable.eulerAngles
            , height      = buildingObjectInfoSerializable.height
        };
    }
}
