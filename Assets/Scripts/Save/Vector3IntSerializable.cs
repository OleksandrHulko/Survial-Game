using System;
using UnityEngine;

[Serializable]
public class Vector3IntSerializable
{
    private int x, y, z;

    public Vector3IntSerializable(Vector3Int vector3Int)
    {
        x = vector3Int.x;
        y = vector3Int.y;
        z = vector3Int.z;
    }
    
    public static implicit operator Vector3IntSerializable(Vector3Int vector3Int)
    {
        return new Vector3IntSerializable(vector3Int);
    }

    public static implicit operator Vector3Int(Vector3IntSerializable vector3IntSerializable)
    {
        return new Vector3Int()
        {
              x = vector3IntSerializable.x
            , y = vector3IntSerializable.y
            , z = vector3IntSerializable.z
        };
    }
}
