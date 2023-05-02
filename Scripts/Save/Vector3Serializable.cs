using System;
using UnityEngine;

[Serializable]
public class Vector3Serializable
{
    private float x, y, z;

    public Vector3Serializable(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }
    
    public static implicit operator Vector3Serializable(Vector3 vector3)
    {
        return new Vector3Serializable(vector3);
    }

    public static implicit operator Vector3(Vector3Serializable vector3Serializable)
    {
        return new Vector3()
        {
              x = vector3Serializable.x
            , y = vector3Serializable.y
            , z = vector3Serializable.z
        };
    }
}
