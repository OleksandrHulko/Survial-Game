using UnityEngine;
using UnityEngine.UI;

public class ObjectCountItem : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Text countText = null;
    [SerializeField]
    private Image icon = null;
    #endregion

    #region Private Fields
    private ObjectType _objectType = ObjectType.None;
    private int _count = 0;
    #endregion
    
    
    #region Public Methods
    public void Init( ObjectTypeIntPair objectTypeIntPair )
    {
        _objectType = objectTypeIntPair.objectType;
        _count = objectTypeIntPair.count;
        
        SetCountText();
        SetIcon();
    }
    #endregion
    
    #region Private Methods
    private void SetCountText()
    {
        countText.text = Helper.GetCountText(_count);
    }

    private void SetIcon()
    {
        icon.sprite = Helper.GetSpriteFromInventoryAtlas(Helper.GetSpriteName(_objectType));
    }
    #endregion
}
