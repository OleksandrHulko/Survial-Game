using UnityEngine;

public class CraftItem : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Transform content = null;
    [SerializeField]
    private InventoryItem inventoryItem = null;
    [SerializeField]
    private CraftRecipeItem craftRecipeItem = null;
    #endregion

    #region Private Fields
    private ObjectTypeIntPairArray[] _resources;
    private ObjectTypeIntPair _result;
    #endregion


    #region Public Methods
    public void Init( CraftRecipe craftRecipe )
    {
        _resources = craftRecipe.nestedRecipes;
        _result = craftRecipe.result;
        
        InitInventoryItem();
        SpawnContent();
    }
    #endregion

    #region Private Methods
    private void InitInventoryItem()
    {
        inventoryItem.Init(_result);
    }

    private void SpawnContent()
    {
        foreach (ObjectTypeIntPairArray objectTypeIntPairArray in _resources)
        {
            Instantiate(craftRecipeItem, content).Init(objectTypeIntPairArray, _result);
        }
    }
    #endregion
}
