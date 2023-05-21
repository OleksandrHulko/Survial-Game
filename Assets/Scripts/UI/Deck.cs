using UnityEngine;

public class Deck : MonoBehaviour , ITake
{
    #region Serialize Fields
    [SerializeField]
    private ObjectType objectType = ObjectType.None;
    #endregion
    
    #region Private Fields
    private float _seconds = 0;
    #endregion
    
    
    #region Private Methods
    private void Start()
    {
        _seconds = 10 + 0.01f * (GetHashCode() % 100); // for a blur in time
    }
    #endregion

    #region Public Methods
    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Take()
    {
        Storage.Add(objectType);
        Destroy();
    }
    #endregion
}
