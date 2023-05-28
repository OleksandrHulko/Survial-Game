using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private ScrollRect scrollRect = null;
    [SerializeField]
    private Text title = null;
    [SerializeField]
    private Button rightButton = null;
    [SerializeField]
    private Button leftButton = null;
    [SerializeField]
    private Transform inventoryContT = null;
    [SerializeField]
    private Transform craftContT = null;
    [SerializeField]
    private CanvasGroup inventoryCanvasG = null;
    [SerializeField]
    private CanvasGroup craftCanvasG = null;
    [SerializeField]
    private InventoryItem inventoryItem = null;
    [SerializeField]
    private CraftItem craftItem = null;
    [SerializeField]
    private CraftRecipesConfig craftRecipesConfig = null;
    #endregion

    #region Private Fields
    private RectTransform _inventoryRectT = null;
    private RectTransform _craftRectT = null;
    private InventoryItem[] _items = null;
    private InventoryTabs _currentTab = InventoryTabs.Inventory;
    private int _tabsCount = 0;
    #endregion

    #region Public Fields
    public static (InventoryItem inventoryItem, ObjectType objectType) selectedItem = (null, ObjectType.None);
    public static Action OnInventoryOpen = null;
    public static Action<ObjectType> OnInventoryClosed = null;
    #endregion
    
    
    #region Private Methods
    private void Awake()
    {
        SpawnInventoryItems();
        SpawnCraftItems();

        _inventoryRectT = inventoryContT.GetComponent<RectTransform>();
        _craftRectT     = craftContT    .GetComponent<RectTransform>();

        _tabsCount = Helper.GetEnumValues<InventoryTabs>().Length;
        
        rightButton.onClick.AddListener(RightBtnClickHandler);
        leftButton .onClick.AddListener(LeftBtnClickHandler);
        
        leftButton.gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        Helper.SetVisibleCursor();
        
        ShowNeededTab();
        
        OnInventoryOpen?.Invoke();
    }

    private void OnDisable()
    {
        Helper.SetVisibleCursor(false);
        
        inventoryCanvasG.SetAlpha(0.0f);
        craftCanvasG.SetAlpha(0.0f);

        OnInventoryClosed?.Invoke(selectedItem.objectType);
    }

    private void SpawnInventoryItems()
    {
        int count = Storage.Objects.Count;
        _items = new InventoryItem[count];

        for (int i = 0; i < count; i++)
        {
            _items[i] = Instantiate(inventoryItem, inventoryContT);
        }
    }

    private void SpawnCraftItems()
    {
        CraftRecipe[] craftRecipes = craftRecipesConfig.GetRecipes();
        foreach (CraftRecipe craftRecipe in craftRecipes)
        {
            Instantiate(craftItem, craftContT).Init(craftRecipe);
        }
    }

    private void ShowNeededTab()
    {
        switch (_currentTab)
        {
            case InventoryTabs.Inventory : ShowInventoryItems();
                break;
            case InventoryTabs.Craft : ShowCraftItems();
                break;
        }
    }

    private void ShowInventoryItems()
    {
        scrollRect.content = _inventoryRectT;
        title.text = Localization.INVENTORY;
        
        StopAllCoroutines();
        StartCoroutine(inventoryCanvasG.SmoothlySetAlpha(1.0f));
        StartCoroutine(craftCanvasG.SmoothlySetAlpha(0.0f));
        TryResetSelectedItem();

        KeyValuePair<ObjectType, int>[] objects = Storage.Objects.Where(x => x.Value != 0).OrderByDescending(x => x.Value).ToArray();

        int index = 0;
        
        for (; index < objects.Length; index++)
        {
            _items[index].Init(objects[index].Key, objects[index].Value);
        }

        for (; index < _items.Length; index++)
        {
            _items[index].SetActive(false);
        }

        void TryResetSelectedItem()
        {
            if (selectedItem.objectType == ObjectType.None)
                return;

            if (Storage.CountOfObjectType(selectedItem.objectType) == 0)
                selectedItem = (null, ObjectType.None);
        }
    }

    private void ShowCraftItems()
    {
        scrollRect.content = _craftRectT;
        title.text = Localization.CRAFT;
        
        StopAllCoroutines();
        StartCoroutine(inventoryCanvasG.SmoothlySetAlpha(0.0f));
        StartCoroutine(craftCanvasG.SmoothlySetAlpha(1.0f));
    }

    private void SwitchTab( bool switchToPreviousTab = false )
    {
        int index = (int)_currentTab;
        index = Mathf.Clamp(switchToPreviousTab ? --index : ++index, 1, _tabsCount);

        _currentTab = (InventoryTabs)index;


        rightButton.gameObject.SetActive(index < _tabsCount);
        leftButton.gameObject.SetActive(index > 1);
    }

    private void RightBtnClickHandler()
    {
        SwitchTab();
        ShowNeededTab();
    }

    private void LeftBtnClickHandler()
    {
        SwitchTab(true);
        ShowNeededTab();
    }
    #endregion
}

public enum InventoryTabs : byte
{
    Inventory = 1 ,
    Craft ,
}