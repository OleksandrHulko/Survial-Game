using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasLoader : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private SpriteAtlas inventoryAtlas = null;
    #endregion

    #region Public Fields
    public static Dictionary<string, Sprite> inventorySprites = null;
    #endregion
    
    #region Private Fields
    #endregion


    #region Private Methods
    private void Awake()
    {
        InitSpritesDictionary();
    }

    private void InitSpritesDictionary()
    {
        ObjectType[] types = Helper.GetRealEnumValues<ObjectType>();
        inventorySprites = new Dictionary<string, Sprite>(types.Length);

        foreach (ObjectType objectType in types)
        {
            string spriteAtlas = Helper.GetSpriteName(objectType);
            inventorySprites.Add(spriteAtlas, inventoryAtlas.GetSprite(spriteAtlas));
        }
    }
    #endregion
}
