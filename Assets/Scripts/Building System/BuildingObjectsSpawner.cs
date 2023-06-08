using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BuildingObjectsSpawner : MonoBehaviour
{
    #region Serialize Fields
    #endregion

    #region Public Fields
    public static List<BuildingObjectInfo> buildingObjectInfos = new List<BuildingObjectInfo>();

    public delegate void GetListCallback(List<BuildingObject> buildingObjects);
    #endregion
    
    #region Private Fields
    private static BuildingObjectsSpawner _instance = null;

    private static Dictionary<Vector2Int, List<BuildingObjectInfo>> _dictionary = new Dictionary<Vector2Int, List<BuildingObjectInfo>>();
    #endregion


    #region Public Methods
    public static void Add(BuildingObjectInfo buildingObjectInfo)
    {
        buildingObjectInfos.Add(buildingObjectInfo);

        if (_dictionary.TryGetValue(buildingObjectInfo.position, out List<BuildingObjectInfo> value))
            value.Add(buildingObjectInfo);
        else
            _dictionary.Add(buildingObjectInfo.position, new List<BuildingObjectInfo> { buildingObjectInfo });
    }

    public static void Remove(BuildingObjectInfo buildingObjectInfo)
    {
        buildingObjectInfos.Remove(buildingObjectInfo);

        _dictionary[buildingObjectInfo.position].Remove(buildingObjectInfo);
    }

    public static bool Overlap(BuildingObjectInfo buildingObjectInfo)
    {
        if (_dictionary.TryGetValue(buildingObjectInfo.position, out List<BuildingObjectInfo> value))
            return value.Contains(buildingObjectInfo);

        return false;
    }

    public static void InitList(Vector2Int position, Transform parentalTransform, List<BuildingObject> buildingObjects, GetListCallback callback)
    {
        Task.Run(() => GetBuildingObjectInfos(position))
            .ContinueWith(
                  task => _instance.StartCoroutine(SpawnBuildingsFromList(task.Result, parentalTransform, buildingObjects, callback))
                , TaskScheduler.FromCurrentSynchronizationContext()
            );
    }
    #endregion
    
    #region Private Methods
    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        InitDictionary();
    }
    
    private void InitDictionary()
    {
        _dictionary = buildingObjectInfos
            .GroupBy(x => x.position)
            .ToDictionary(x => x.Key, x => x.ToList());
    }

    private static List<BuildingObjectInfo> GetBuildingObjectInfos(Vector2Int chunkVirtualPosition)
    {
        int chunkWidth = SettingsSaver.chunkWidth;
        
        int xInitial = chunkVirtualPosition.x;
        int yInitial = chunkVirtualPosition.y;
        
        int xFinal = xInitial + chunkWidth;
        int yFinal = yInitial + chunkWidth;
        
        Vector2Int currentIteration = Vector2Int.zero;

        List<BuildingObjectInfo> objectsLocal = new List<BuildingObjectInfo>();

        for (int x = xInitial; x < xFinal; x++)
        {
            for (int y = yInitial; y < yFinal; y++)
            {
                currentIteration.x = x;
                currentIteration.y = y;

                if (_dictionary.TryGetValue(currentIteration, out List<BuildingObjectInfo> value))
                    objectsLocal.AddRange(value);
            }
        }

        return objectsLocal;
    }

    private static IEnumerator SpawnBuildingsFromList(List<BuildingObjectInfo> objects, Transform parentalTransform, List<BuildingObject> buildingObjects, GetListCallback callback)
    {
        foreach (BuildingObject buildingObject in buildingObjects)
        {
            if (buildingObject)
                buildingObject.Destroy();
        }

        buildingObjects = new List<BuildingObject>(objects.Count);

        for (int i = 0; i < objects.Count; i++)
        {
            BuildingObjectInfo objectInfo = objects[i];
            
            buildingObjects.Add(Instantiate(
                PrefabsStorage.GetBuildingObject(objectInfo.objectType)
                , objectInfo.GetRealPosition()
                , Quaternion.Euler(objectInfo.eulerAngles)
                , parentalTransform)
            );

            if (i % 10 == 0)
                yield return null;
        }

        callback.Invoke(buildingObjects);
    }

    #endregion
}

public struct BuildingObjectInfo
{
    public ObjectType objectType;
    public Vector2Int position;
    public Vector3Int eulerAngles;
    public int height;

    public BuildingObjectInfo(ObjectType objectType, Vector3 position, Vector3Int eulerAngles)
    {
        this.objectType  = objectType;
        this.position    = position.ToVector2Int();
        this.eulerAngles = eulerAngles;
        this.height      = Mathf.RoundToInt(position.y);
    }

    public Vector3 GetPosition()
    {
        return new Vector3(position.x, height, position.y);
    }
    
    public Vector3 GetRealPosition()
    {
        return position.ToRealPosition(height);
    }
    
    public static explicit operator BuildingObjectInfo(BuildingObject buildingObject)
    {
        Transform objectTransform = buildingObject.transform;
        
        return new BuildingObjectInfo()
        {
              objectType = buildingObject.ObjectType
            , position = objectTransform.position.ToVirtualPosition().ToVector2Int()
            , eulerAngles = Vector3Int.CeilToInt(objectTransform.eulerAngles)
            , height = Mathf.RoundToInt(objectTransform.position.y)
        };
    }
}