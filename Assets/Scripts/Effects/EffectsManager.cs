using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private AxCutting axCutting = null;
    #endregion

    #region Public Fields
    public static EffectsManager Instance => _instance;
    #endregion
    
    #region Private Fields
    private static EffectsManager _instance = null;
    #endregion


    #region Private Methods
    private void Awake()
    {
        _instance = this;
    }
    #endregion
    
    #region Public Methods
    public void PlayAxCutting( Vector3 position )
    {
        axCutting.Play(position);
    }
    #endregion
}
