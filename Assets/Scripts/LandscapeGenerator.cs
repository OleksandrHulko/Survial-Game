using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeGenerator : MonoBehaviour
{
    #region Serialize Fields
    [Header("Render distance in chunks")]
    [Range(0,byte.MaxValue)]
    [SerializeField]
    private byte renderDistance = 1;
    [Space]
    [Header("Width of chunk in meters")]
    [SerializeField]
    private short chunkWidth;
    [Header("Max landscape height in meters")]
    [SerializeField]
    private short chunkHeight;
    [Space]
    [Header("Chunk prefab")]
    [SerializeField]
    private Chunk chunkPrefab = null;
    [Space]
    [Header("Landscape settings config")]
    [SerializeField]
    private LandscapeSettingsConfig landscapeSettingsConfig = null;
    [Space]
    [Header("Player transform")]
    [SerializeField]
    private Transform playerTransform = null;
    #endregion
    
    #region Private Fields
    private Dictionary<Vector2Int, Chunk> _chunksDictionary = null;
    private Vector2Int _offset = Vector2Int.zero;
    private static LandscapeGenerator _landscapeGenerator = null;
    #endregion
    
    
    #region Private Methods

    private void Awake()
    {
        _landscapeGenerator = this;
    }

    private void Start()
    {
        _offset = SaveManager.GetInstance().GetPlayerPosition().ToVector2Int();
        _chunksDictionary = new Dictionary<Vector2Int, Chunk>();
        SpawnChunks(_offset);
        StartCoroutine(RespawnChunks());
    }

    private void SpawnChunks(Vector2Int center)
    {
        for (int z = -renderDistance; z < renderDistance + 1; z++)
        {
            for (int x = -renderDistance; x < renderDistance + 1; x++)
            {
                Vector2Int vector2Int = new Vector2Int((x * chunkWidth) + center.x, (z * chunkWidth) + center.y);
                
                if (Vector2.Distance(vector2Int, center) > renderDistance * chunkWidth)
                    continue;

                Chunk chunk = Instantiate(chunkPrefab, transform);
                chunk.Init(landscapeSettingsConfig, vector2Int, chunkWidth, chunkHeight);

                _chunksDictionary.Add(vector2Int, chunk);
            }
        }
    }

    private IEnumerator RespawnChunks()
    {
        List<Vector2Int> spawn = new List<Vector2Int>();
        List<Vector2Int> despawn = new List<Vector2Int>();

        while (true)
        {
            yield return null;
            
            if (ResetPositionManager.needDisplacement)
                ResetPositionManager.Displace();
            
            yield return Respawn();
        }
        
        IEnumerator Respawn()
        {
            Vector2Int position = GetNearestChunkPos(playerTransform.position.ToVirtualPosition().ToVector2Int());
            
            for (int z = -renderDistance; z < renderDistance + 1; z++)
            {
                for (int x = -renderDistance; x < renderDistance + 1; x++)
                {
                    Vector2Int vector2Int = new Vector2Int((x * chunkWidth) + position.x, (z * chunkWidth) + position.y);
                    
                    bool isFarChunk = Vector2.Distance(vector2Int, position) > renderDistance * chunkWidth;
                    bool containsKey = _chunksDictionary.ContainsKey(vector2Int);

                    if (!isFarChunk && !containsKey)
                        spawn.Add(vector2Int);
                }
            }

            if (spawn.Count == 0)
                yield break;

            foreach (Vector2Int key in _chunksDictionary.Keys)
            {
                if (Vector2.Distance(key, position) > renderDistance * chunkWidth)
                    despawn.Add(key);
            }

            for (int i = 0; i < despawn.Count; i++)
            {
                Chunk chunk = _chunksDictionary[despawn[i]];
                
                _chunksDictionary.Remove(despawn[i]);
                _chunksDictionary.Add(spawn[i], chunk);
            }
            
            foreach (KeyValuePair<Vector2Int, Chunk> chunk in _chunksDictionary)
            {
                chunk.Value.SetNeighbors(GetNeighbors(chunk.Key));
                if (spawn.Contains(chunk.Key))
                {
                    yield return chunk.Value.Reinit(chunk.Key);
                    yield return null;
                }
            }

            for (int i = 0; i < spawn.Count; i++)
            {
                _chunksDictionary[spawn[i]].ReinitLiteNeighbors();
                yield return null;
            }
            
            if(spawn.Count != despawn.Count)
                Debug.LogWarning($"spawn count: {spawn.Count} | despawn.Count: {despawn.Count}");
        
            spawn.Clear();
            despawn.Clear();
        }
    }

    private Vector2Int GetNearestChunkPos( Vector2Int objectPosition )
    {
        int wholeX = (objectPosition.x - _offset.x) / chunkWidth;
        int wholeY = (objectPosition.y - _offset.y) / chunkWidth;

        int x = (objectPosition.x - _offset.x >= 0 ? wholeX : wholeX - 1) * chunkWidth;
        int y = (objectPosition.y - _offset.y >= 0 ? wholeY : wholeY - 1) * chunkWidth;
        
        return new Vector2Int(_offset.x + x, _offset.y + y);
    }

    private Chunk[] GetNeighbors( Vector2Int position )
    {
        Chunk[] neighbors = new Chunk[4];
        
        if (_chunksDictionary.TryGetValue(position + Vector2Int.left * chunkWidth, out Chunk chunkLeft))
        {
            neighbors[0] = chunkLeft;
        }
        if (_chunksDictionary.TryGetValue(position + Vector2Int.up * chunkWidth, out Chunk chunkTop))
        {
            neighbors[1] = chunkTop;
        }
        if (_chunksDictionary.TryGetValue(position + Vector2Int.right * chunkWidth, out Chunk chunkRight))
        {
            neighbors[2] = chunkRight;
        }
        if (_chunksDictionary.TryGetValue(position + Vector2Int.down * chunkWidth, out Chunk chunkBottom))
        {
            neighbors[3] = chunkBottom;
        }

        return neighbors;
    }
    #endregion

    #region Public Methods
    public static LandscapeGenerator GetInstance()
    {
        return _landscapeGenerator;
    }

    public float GetSurfaceHeight( Vector2Int position )
    {
        return landscapeSettingsConfig.GetHeight(position) * chunkHeight;
    }
    #endregion
}

#region Enum
public enum BiomeType
{
    None = 0 ,
    Water    ,
    Sand     ,
    Grass    ,
    Rock     ,
    Snow     ,
    
}
#endregion
