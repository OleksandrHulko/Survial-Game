using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class TreeSpawner : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Tree palm    = null;
    [SerializeField]
    private Tree oak     = null;
    [SerializeField]
    private Tree juniper = null;
    [SerializeField]
    private Tree pine    = null;
    #endregion

    #region Private Fields
    private static TreeSpawner _instance = null;

    private static Tree[] _palmsPool    = null;
    private static Tree[] _oaksPool     = null;
    private static Tree[] _junipersPool = null;
    private static Tree[] _pinesPool    = null;
    
    private static ushort _eachTypeTreeCountInPool = 1_000;
    private static ushort _palmsPoolIdx    = 0;
    private static ushort _oaksPoolIdx     = 0;
    private static ushort _junipersPoolIdx = 0;
    private static ushort _pinesPoolIdx    = 0;
    
    private const int DISTANCE_BETWEEN_TREES = 50;
    #endregion

    #region Public Fields
    public static HashSet<Vector2Int> felledTreesPositions = new HashSet<Vector2Int>();
    #endregion


    #region Private Methods
    private void Awake()
    {
        int a = 1 + 1;
        int b = 1 + 1;
        int c = 1 + 1;
        int d = 1 + 1;
        int abcd = 1 + 1;
        
        _instance = this;
        InitPool();
    }

    private void OnEnable()
    {
        SetSavedFelledTreesPositions();
    }

    private void InitPool()
    {
        TreeType[] treeTypes = (TreeType[]) Enum.GetValues(typeof(TreeType));

        _palmsPool    = new Tree[_eachTypeTreeCountInPool];
        _oaksPool     = new Tree[_eachTypeTreeCountInPool];
        _junipersPool = new Tree[_eachTypeTreeCountInPool];
        _pinesPool    = new Tree[_eachTypeTreeCountInPool];

        foreach (TreeType treeType in treeTypes)
        {
            if (treeType == TreeType.None)
                continue;
            
            for (int i = 0; i < _eachTypeTreeCountInPool; i++)
            {
                Tree tree = Instantiate(GetSerializeTree(treeType), transform);
                tree.gameObject.SetActive(false);
                
                switch (treeType)
                {
                    case TreeType.Palm    : _palmsPool   [i] = tree; break;
                    case TreeType.Oak     : _oaksPool    [i] = tree; break;
                    case TreeType.Juniper : _junipersPool[i] = tree; break;
                    case TreeType.Pine    : _pinesPool   [i] = tree; break;
                }
            }
        }
    }
    
    private Tree GetSerializeTree(TreeType treeType)
    {
        switch (treeType)
        {
            case TreeType.Palm    : return palm;
            case TreeType.Oak     : return oak;
            case TreeType.Juniper : return juniper;
            case TreeType.Pine    : return pine;
                
            default: throw new InvalidEnumArgumentException($"Not case for {treeType}");
        }
    }

    private static Tree GetTree(TreeType treeType)
    {
        Tree tree = null;
            
        switch (treeType)
        {
            case TreeType.Palm :
                tree = _palmsPool[_palmsPoolIdx];
                Increment(ref _palmsPoolIdx);
                break;
            case TreeType.Oak :
                tree = _oaksPool[_oaksPoolIdx];
                Increment(ref _oaksPoolIdx);
                break;
            case TreeType.Juniper :
                tree = _junipersPool[_junipersPoolIdx];
                Increment(ref _junipersPoolIdx);
                break;
            case TreeType.Pine :
                tree = _pinesPool[_pinesPoolIdx];
                Increment(ref _pinesPoolIdx);
                break;
        }

        return tree;
    }

    private static void Increment( ref ushort value )
    {
        value++;
        
        if (value == _eachTypeTreeCountInPool)
            value = 0;
    }

    private static void SetSavedFelledTreesPositions()
    {
        felledTreesPositions = SaveManager.GetInstance().felledTreesPositions;
    }
    #endregion
    
    #region Public Methods
    public static TreeSpawner GetInstance() // TODO delete it and maybe made this class static
    {
        return _instance;
    }

    public static void AddNewFelledTreesPositions(Vector2Int position)
    {
        felledTreesPositions.Add(position);
    }

    public static void InitTrees( Transform parentalTransform, Vector2Int chunkVirtualPosition, LandscapeSettingsConfig lSC, float[,] heights, ref List<Tree> treesInChunk )
    {
        int chunkVPX = chunkVirtualPosition.x;
        int chunkVPY = chunkVirtualPosition.y;
        int x = 0;
        int y = 0;
        int xInitial = (DISTANCE_BETWEEN_TREES - (chunkVPX % DISTANCE_BETWEEN_TREES)) % DISTANCE_BETWEEN_TREES; // find iteration X which lies on grid DISTANCE_BETWEEN_TREES x DISTANCE_BETWEEN_TREES in virtual coordinates.
        int yInitial = (DISTANCE_BETWEEN_TREES - (chunkVPY % DISTANCE_BETWEEN_TREES)) % DISTANCE_BETWEEN_TREES; // find iteration Y which lies on grid DISTANCE_BETWEEN_TREES x DISTANCE_BETWEEN_TREES in virtual coordinates.
        int xDisplacement = 0;
        int yDisplacement = 0;
        int width = Settings.chunkWidth;
        int listTreesCapacity = (int) Mathf.Pow(((width / DISTANCE_BETWEEN_TREES) + 1), 2);
        float height = 0.0f;
        
        List<Tree> trees = new List<Tree>(listTreesCapacity);

        for (y = yInitial; y < width; y += DISTANCE_BETWEEN_TREES)
        {
            for (x = xInitial; x < width; x += DISTANCE_BETWEEN_TREES)
            {
                if (CanPlacingTree())
                    PlacingTree();
            }
        }

        foreach (Tree tree in treesInChunk)
        {
            tree.HideTreeOnChunk();
        }

        treesInChunk = trees;
        
        bool CanPlacingTree()
        {
            int gridVirtualPosX = chunkVPX + x;
            int gridVirtualPosY = chunkVPY + y;

            int displaceRange = DISTANCE_BETWEEN_TREES - 10;

            xDisplacement = x + (displaceRange / 2 - ((gridVirtualPosX + gridVirtualPosY ) % displaceRange));
            yDisplacement = y + (displaceRange / 2 - ((gridVirtualPosX - gridVirtualPosY ) % displaceRange));

            bool outOfRange = xDisplacement < 0 || xDisplacement >= width || yDisplacement < 0 || yDisplacement >= width;

            height = outOfRange
                ? lSC.GetHeight(new Vector2Int(chunkVPY + yDisplacement, chunkVPX + xDisplacement)).GetRealHeightFromTerrainHeight()
                : heights[yDisplacement, xDisplacement].GetRealHeightFromTerrainHeight();

            return ((((int) height) % 100) / 10) % 2 == 0 && !TreeIsFelled(); // the parity of the 10m digit

            bool TreeIsFelled() => felledTreesPositions.Contains(new Vector2Int(chunkVPX + xDisplacement, chunkVPY + yDisplacement));
        }

        void PlacingTree()
        {
            int seed = (int) (height * 100);
            Tree tree = null;
            BiomeType biomeType = height.GetBiomesType();

            switch (biomeType)
            {
                case BiomeType.Water :
                    return;
                case BiomeType.Sand :
                    tree = GetTree(TreeType.Palm);
                    break;
                case BiomeType.Grass :
                    tree = 70.GetTrueBoolWithProbability(seed) ? GetTree(TreeType.Oak) : GetTree(TreeType.Juniper);
                    break;
                case BiomeType.Rock :
                    tree = 50.GetTrueBoolWithProbability(seed) ? GetTree(TreeType.Juniper) : GetTree(TreeType.Pine);
                    break;
                case BiomeType.Snow :
                    tree = GetTree(TreeType.Pine);
                    break;
            }

            trees.Add(tree);
            tree.ShowTreeOnChunk(parentalTransform, new Vector3(xDisplacement, height, yDisplacement));
        }
    }
    #endregion
}
