using System;
using System.Collections.Generic;

public static class Storage
{
    #region Private Fields
    private static Dictionary<ObjectType, int> _objects = null;
    #endregion

    #region Public Fields
    public static Dictionary<ObjectType, int> Objects
    {
        get => _objects;
        set => _objects = value ?? InitEmptyDictionary();
    }
    #endregion


    #region Public Methods
    public static int CountOfObjectType(ObjectType objectType)
    {
        return _objects[objectType];
    }
    
    public static void Add(ObjectTypeIntPair objectTypeIntPair)
    {
        Add(objectTypeIntPair.objectType, objectTypeIntPair.count);
    }

    public static void Add(ObjectType objectType, int count = 1)
    {
        int newCount = _objects[objectType] + count;
        _objects[objectType] = newCount;
    }

    public static void TryRemove(ObjectType objectType, int count, out bool successfully)
    {
        int newCount = _objects[objectType] - count;
        successfully = newCount >= 0;

        if (successfully)
            _objects[objectType] = newCount;
    }
    
    public static void TryRemove(ObjectTypeIntPairArray objectTypeIntPairArray, out bool successfully)
    {
        ObjectTypeIntPair[] objectTypeIntPairs = objectTypeIntPairArray.resources;
        int lenght = objectTypeIntPairs.Length;
        int[] newCounts = new int[lenght];
        successfully = true;

        for (int i = 0; i < lenght; i++)
        {
            ObjectTypeIntPair objectTypeIntPair = objectTypeIntPairs[i];
            newCounts[i] = _objects[objectTypeIntPair.objectType] - objectTypeIntPair.count;
        }

        for (int i = 0; i < lenght; i++)
        {
            if (newCounts[i] < 0)
            {
                successfully = false;
                return;
            }
        }

        for (int i = 0; i < lenght; i++)
            _objects[objectTypeIntPairs[i].objectType] = newCounts[i];
    }
    #endregion

    #region Private Methods
    private static Dictionary<ObjectType, int> InitEmptyDictionary()
    {
        ObjectType[] objectTypes = Helper.GetRealEnumValues<ObjectType>();
        int capacity = objectTypes.Length;
        Dictionary<ObjectType, int> dictionary = new Dictionary<ObjectType, int>(capacity);

        foreach (ObjectType objectType in objectTypes)
            dictionary[objectType] = 0;

        return dictionary;
    }
    #endregion
}
