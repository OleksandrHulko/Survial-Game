using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Private Fields
    private static SaveManager _instance = null;
    private string _filePath = string.Empty;
    #endregion
    
    #region Public Fields
    public Vector3? playerPosition = null;
    public float playerYEulerAngle = 0.0f;
    public HashSet<Vector2Int> felledTreesPositions = new HashSet<Vector2Int>();
    public int days = 0;
    public float dayCycle = 0;
    //public Dictionary<ObjectType, int> objects = new Dictionary<ObjectType, int>();

    #endregion

    #region Private Methods
    private void Awake()
    {
        _instance = this;
        _filePath = $"{Application.persistentDataPath}/SaveData.dat";
        LoadGameData();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
            SaveGameData();
        if(Input.GetKeyDown(KeyCode.F2))
            LoadGameData();
        if(Input.GetKeyDown(KeyCode.F3))
            CheckValues();
        
        if(Input.GetKeyDown(KeyCode.F12))
            DeleteGameData();
    }
    
    private void OnApplicationQuit()
    {
        SaveGameData();
    }
    #endregion
    
    #region Public Methods
    public void SaveGameData()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(_filePath);
        SaveData saveData = new SaveData
        {
              playerPosition = Controller.GetInstance().GetPosition().ToVirtualPosition()
            , playerYEulerAngle = Controller.GetInstance().GetRotation().eulerAngles.y
            , felledTreesPositions = new HashSet<Vector2IntSerializable>(TreeSpawner.felledTreesPositions.Select(x=> new Vector2IntSerializable(x)))
            , days = TimeManager.GetInstance().GetTimeValues().days
            , dayCycle = TimeManager.GetInstance().GetTimeValues().dayCycle
            , objects = Storage.Objects
            , buildingObjects = new List<BuildingObjectInfoSerializable>(BuildingObjectsSpawner.buildingObjectInfos.Select(x=>(BuildingObjectInfoSerializable)x))
        };
        binaryFormatter.Serialize(fileStream, saveData);
        fileStream.Close();
        Debug.Log("Save game data");
    }

    public void LoadGameData()
    {
        if (File.Exists(_filePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(_filePath, FileMode.Open);
            SaveData saveData = (SaveData) binaryFormatter.Deserialize(fileStream);
            playerPosition = saveData.playerPosition;
            playerYEulerAngle = saveData.playerYEulerAngle;
            felledTreesPositions = new HashSet<Vector2Int>(saveData.felledTreesPositions.Select(x => (Vector2Int) x));
            days = saveData.days;
            dayCycle = saveData.dayCycle;
            Storage.Objects = saveData.objects;
            BuildingObjectsSpawner.buildingObjectInfos = new List<BuildingObjectInfo>(saveData.buildingObjects.Select(x => (BuildingObjectInfo)x));
            fileStream.Close();
            Debug.Log("Load game data");
        }
        else
        {
            Storage.Objects = null;
            BuildingObjectsSpawner.buildingObjectInfos = new List<BuildingObjectInfo>();
            Debug.Log("File not found");
        }
    }

    public void DeleteGameData()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
            Debug.Log("Deleted game data");
        }
        else
            Debug.Log("File not found");
    }

    public static SaveManager GetInstance()
    {
        return _instance;
    }

    public void CheckValues()
    {
        Debug.Log($"{Storage.Objects.Count}");
        foreach (var value in Storage.Objects)
        {
            Debug.Log(value);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition ?? Vector3.zero;
    }
    #endregion
}
