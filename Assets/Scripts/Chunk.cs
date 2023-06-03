using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField] 
    private Terrain terrain = null;
    [SerializeField]
    private TerrainCollider terrainCollider = null;
    [SerializeField]
    private Material material = null;
    #endregion
    
    #region Private Fields
    private TerrainData _terrainData = null;
    private LandscapeSettingsConfig _landscapeSettingsConfig = null;
    private Vector2Int _position;
    private Chunk _leftNeighbor = null;
    private Chunk _topNeighbor = null;
    private Chunk _rightNeighbor = null;
    private Chunk _bottomNeighbor = null;
    private short _width;
    private short _height;
    private float[,] _heights;

    private List<Tree> _trees = new List<Tree>();//set capacity
    private List<BuildingObject> _buildingObjects = new List<BuildingObject>();
    private byte _maxTreeCount = 128;
    #endregion


    #region Public Methods
    public void Init( LandscapeSettingsConfig landscapeSettingsConfig, Vector2Int position, short width, short height )
    {
        _landscapeSettingsConfig = landscapeSettingsConfig;
        _position = position;
        _width = width;
        _height = height;
        
        _terrainData = new TerrainData();
        terrain.terrainData = _terrainData;
        terrainCollider.terrainData = _terrainData;
        
        terrain.materialTemplate = material;
        _terrainData.SetDetailResolution(_width, _width);

        _terrainData.heightmapResolution = _width+1;
        _terrainData.size = new Vector3(_width, _height, _width);

        _heights = _landscapeSettingsConfig.GetHeights(position.y, position.x, _width + 1);
        _terrainData.SetHeights(0, 0, _heights);
        
        terrain.transform.position = position.ToRealPosition();
        
        SetTreesPosition();
        SpawnBuildings();
        
        ResetPositionManager.AddTransform(transform);
    }

    public IEnumerator Reinit( Vector2Int position )
    {
        _position = position;

        yield return _landscapeSettingsConfig.InitHeights(position.y, position.x, _width + 1);
        _heights = _landscapeSettingsConfig.Heights;
        _terrainData.SetHeights(0,0, _heights);
        terrain.transform.position = position.ToRealPosition();
        
        SetTreesPosition();
        SpawnBuildings();
    }

    public void SetNeighbors(Chunk[] neighbors)
    {
        _leftNeighbor   = neighbors[0];
        _topNeighbor    = neighbors[1];
        _rightNeighbor  = neighbors[2];
        _bottomNeighbor = neighbors[3];


        terrain.SetNeighbors(neighbors[0]?.terrain, neighbors[1]?.terrain, neighbors[2]?.terrain, neighbors[3]?.terrain);
    }
    
    public void ReinitLiteNeighbors()
    {
        if (_leftNeighbor)
            _leftNeighbor.ReinitLite();

        if (_topNeighbor)
            _topNeighbor.ReinitLite();

        if (_rightNeighbor)
            _rightNeighbor.ReinitLite();

        if (_bottomNeighbor)
            _bottomNeighbor.ReinitLite();
    }

    public void AddNewBuildingObjects( BuildingObject buildingObject )
    {
        _buildingObjects.Add(buildingObject);
    }
    #endregion
    
    #region Private Methods
    private void ReinitLite()
    {
        _terrainData.SetHeights(0,0, new float[0,0]);
    }

    private void SetTreesPosition()
    {
        TreeSpawner.InitTrees(transform, _position, _landscapeSettingsConfig, _heights, ref _trees);
    }

    private void SpawnBuildings()
    {
        BuildingObjectsSpawner.InitList(_position, transform, _buildingObjects, ReinitBuildingObjectsList);
    }

    private void ReinitBuildingObjectsList(List<BuildingObject> buildingObjects)
    {
        _buildingObjects = buildingObjects;
    }
    #endregion
}
