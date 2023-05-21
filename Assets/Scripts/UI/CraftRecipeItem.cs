using UnityEngine;
using UnityEngine.UI;

public class CraftRecipeItem : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Transform content = null;
    [SerializeField]
    private Button craftButton = null;
    [SerializeField]
    private Text craftBtnText = null;
    [SerializeField]
    private ObjectCountItem objectCountItem = null;
    #endregion

    #region Private Fields
    private ObjectTypeIntPairArray _objectTypeIntPairArray;
    private ObjectTypeIntPair _objectTypeIntPair;
    #endregion


    #region Public Methods
    public void Init( ObjectTypeIntPairArray objectTypeIntPairArray, ObjectTypeIntPair objectTypeIntPair )
    {
        _objectTypeIntPairArray = objectTypeIntPairArray;
        _objectTypeIntPair = objectTypeIntPair;
        
        SpawnItems();
        InitButton();
    }
    #endregion

    #region Private Methods
    private void SpawnItems()
    {
        foreach (ObjectTypeIntPair objectTypeIntPair in _objectTypeIntPairArray.resources)
            Instantiate(objectCountItem, content).Init(objectTypeIntPair);
    }
    
    private void InitButton()
    {
        craftBtnText.text = Localization.CRAFTING;
        craftButton.onClick.AddListener(CraftButtonHandler);
    }

    private void CraftButtonHandler()
    {
        Storage.TryRemove(_objectTypeIntPairArray, out bool successfully);

        if (successfully)
            Storage.Add(_objectTypeIntPair);
        else
            Tooltip.InitStatic(Localization.NO_RESOURCES);
    }
    #endregion
}
