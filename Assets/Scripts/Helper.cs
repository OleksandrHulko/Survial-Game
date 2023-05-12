using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    public static void SetAlpha(this CanvasGroup canvasGroup, float alpha)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = alpha > 0.5f;
    }

    public static IEnumerator SmoothlySetAlpha(this CanvasGroup canvasGroup, float targetAlpha, float seconds = 1.0f)
    {
        float startAlpha = canvasGroup.alpha;
        float lerpValue = 0.0f;
        
        canvasGroup.blocksRaycasts = targetAlpha > 0.5f;
        
        while (Math.Abs(canvasGroup.alpha - targetAlpha) > 0.001f)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerpValue);
            lerpValue += Time.unscaledDeltaTime / seconds;
            
            yield return null;
        }
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

    #region String Helper
    public static string GetObjectName(ObjectType objectType)
    {
        switch (objectType)
        {
            case ObjectType.None        : return Localization.NONE;
            case ObjectType.PalmDeck    : return Localization.PALM_DECK;
            case ObjectType.OakDeck     : return Localization.OAK_DECK;
            case ObjectType.JuniperDeck : return Localization.JUNIPER_DECK;
            case ObjectType.PineDeck    : return Localization.PINE_DECK;
            
            default: throw new InvalidEnumArgumentException($"Not case for {objectType}");
        }
    }

    public static string GetCountText(int count)
    {
        return $"x{count}";
    }
    
    public static string GetSpriteName(ObjectType objectType)
    {
        switch (objectType)
        {
            case ObjectType.PalmDeck    : return "PalmDeck";
            case ObjectType.OakDeck     : return "OakDeck";
            case ObjectType.JuniperDeck : return "JuniperDeck";
            case ObjectType.PineDeck    : return "PineDeck";
            
            default: throw new InvalidEnumArgumentException($"Not case for {objectType}");
        }
    }
    #endregion

    #region Enum Helper
    public static T[] GetEnumValues<T>() where T : Enum
    {
        return (T[])Enum.GetValues(typeof(T));
    }
    
    public static T[] GetRealEnumValues<T>() where T : Enum // without None
    {
        T[] allValues = (T[])Enum.GetValues(typeof(T));
        int lenght = allValues.Length;
        T[] allRealValues = new T[lenght - 1];

        for (int i = 0; i < lenght - 1; i++)
            allRealValues[i] = allValues[i + 1];
        
        return allRealValues;
    }
    #endregion

    #region Sprite Atlas Helper
    public static Sprite GetSpriteFromInventoryAtlas(string name)
    {
        return SpriteAtlasLoader.inventorySprites[name];
    }
    #endregion

    #region Other
    public static void SetVisibleCursor(bool visible = true)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
    #endregion
}