using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabsStorage", menuName = "ScriptableObjects/CreateNewPrefabsStorage")]

public class PrefabsStorage : ScriptableObject
{
    #region Serialize Fields
    [SerializeField]
    private ObjectTypeBuildingObjectPair[] pairs = null;
    #endregion

    #region Private Fields
    private static Dictionary<ObjectType, BuildingObject> _dictionary = null;
    #endregion


    #region Public Methods
    public void Init()
    {
        _dictionary = pairs.ToDictionary(x => x.objectType, x=> x.buildingObject);
    }

    public static BuildingObject GetBuildingObject(ObjectType objectType)
    {
        return _dictionary[objectType];
    }
    #endregion
}

[Serializable]
public class ObjectTypeBuildingObjectPair
{
    public ObjectType objectType;
    public BuildingObject buildingObject;
}