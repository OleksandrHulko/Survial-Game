using System;
using UnityEngine;

[Serializable]
public class Vector2IntSerializable
{
    private int x, y;

    public Vector2IntSerializable(Vector2Int vector2Int)
    {
        x = vector2Int.x;
        y = vector2Int.y;
    }

    public static implicit operator Vector2IntSerializable(Vector2Int vector2Int)
    {
        return new Vector2IntSerializable(vector2Int);
    }

    public static implicit operator Vector2Int(Vector2IntSerializable vector2IntSerializable)
    {
        return new Vector2Int()
        {
              x = vector2IntSerializable.x
            , y = vector2IntSerializable.y
        };
    }
}
