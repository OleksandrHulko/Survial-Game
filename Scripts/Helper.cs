using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class Helper
{
    #region Extensions Methods
    public static void ClampAngle(this ref float value, float min, float max)
    {
        value = value.ClampedAngle(min, max);
    }

    /// <para>This method add values to euler angles. Use (x/y/z)SetMode to override original values</para>
    public static void CorrectEulerAngles(this Transform transform, float x = 0, float y = 0, float z = 0, bool xSetMode = false, bool ySetMode = false, bool zSetMode = false)
    {
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler
        (
            xSetMode ? x : eulerAngles.x + x,
            ySetMode ? y : eulerAngles.y + y,
            zSetMode ? z : eulerAngles.z + z
        );
    }

    public static void SetActiveInverse(this GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public static void SetXPosition(this Transform transform, float x)
    {
        Vector3 position = transform.position;
        position.x = x;
        transform.position = position;
    }
    
    public static void SetYPosition(this Transform transform, float y)
    {
        Vector3 position = transform.position;
        position.y = y;
        transform.position = position;
    }
    
    public static void SetZPosition(this Transform transform, float z)
    {
        Vector3 position = transform.position;
        position.z = z;
        transform.position = position;
    }

    public static float ClampedAngle(this float value, float min, float max)
    {
        if (value < 0.0f)
            value = 360.0f + value;

        return value > 180.0f
            ? Mathf.Clamp(value, 360.0f - max, 360.0f)
            : Mathf.Clamp(value, 0, -min);
    }

    public static float GetRealHeightFromTerrainHeight(this float height)
    {
        return height * Settings.chunkHeight;
    }

    public static bool GetTrueBoolWithProbability(this int probability, int seed)
    {
        Random random = new Random(seed);
        return random.Next(1, 101) <= probability;// CHEK THIS
    }

    public static Vector3 ToVirtualPosition(this Vector3 position)
    {
        Vector2Int offset = ResetPositionManager.offset;

        position.x += offset.x;
        position.z += offset.y;
        
        return position;
    }

    public static Vector3 ToRealPosition(this Vector2Int vector2Int)
    {
        Vector2Int offset = ResetPositionManager.offset;
        return new Vector3(vector2Int.x - offset.x, 0.0f, vector2Int.y - offset.y);
    }

    public static Vector2Int ToVector2Int(this Vector3 vector3)
    {
        return new Vector2Int(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.z));
    }

    public static BiomeType GetBiomesType(this float height)
    {
        if (height <= Settings.maxWaterLevel)
            return BiomeType.Water;
        
        if (height <= Settings.maxSandLevel)
            return BiomeType.Sand;
        
        if (height <= Settings.maxGrassLevel)
            return BiomeType.Grass;
        
        if (height <= Settings.maxRockLevel)
            return BiomeType.Rock;

        return BiomeType.Snow;
    }
    #endregion

    #region Damage System Helper
    public static readonly Dictionary<DamageDamageablePair, int> DamageValues = new Dictionary<DamageDamageablePair, int>
    {
        {new DamageDamageablePair(DamageType.Ax, DamageableType.Tree), 4}
    };
    #endregion
}
