using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Text objectName = null;
    [SerializeField]
    private Text countTxt = null;
    [SerializeField]
    private Text checkmarkTxt = null;
    [SerializeField]
    private Image icon = null;
    #endregion

    #region Private Fields
    private static readonly Color _whiteTransparent = new Color(1f, 1f, 1f, 0.3922f);
    private ObjectType _objectType = ObjectType.None;
    private int _count = 0;
    private string _name = string.Empty;
    #endregion


    #region Public Methods
    public void Init(ObjectTypeIntPair objectTypeIntPair)
    {
        Init(objectTypeIntPair.objectType, objectTypeIntPair.count);
    }
    
    public void Init( ObjectType objectType, int count )
    {
        _objectType = objectType;
        _count = count;
        
        _name = Helper.GetObjectName(_objectType);

        if (Inventory.selectedItem.objectType == _objectType)
            OnClick();
        else
            SetSelected(false);
        
        SetImage();
        SetName();
        SetCountText();
        SetActive();
    }

    public void SetActive(bool active = true)
    {
        gameObject.SetActive(active);
    }

    public void OnClick()
    {
        (InventoryItem inventoryItem, ObjectType objectType) selectedItem = Inventory.selectedItem;
        
        if (selectedItem.objectType != ObjectType.None)
            selectedItem.inventoryItem.SetSelected(false);

        Inventory.selectedItem = (this, _objectType);
        
        SetSelected();

        if (_objectType.CanUseForBuild())
            Player.SetPlayerState(PlayerStatE.Build);
        else
            Player.SetPlayerState(PlayerStatE.Busy);
    }
    #endregion

    #region Private Methods
    private void SetImage()
    {
        icon.sprite = Helper.GetSpriteFromInventoryAtlas(Helper.GetSpriteName(_objectType));
    }
    
    private void SetName()
    {
        objectName.text = _name;
    }

    private void SetCountText()
    {
        countTxt.text = Helper.GetCountText(_count);
    }

    private void SetSelected( bool select = true )
    {
        checkmarkTxt.gameObject.SetActive(select);
        icon.color = select ? _whiteTransparent : Color.white;
    }
    #endregion
}
