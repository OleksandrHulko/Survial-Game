using UnityEngine;

public class UI : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Console console = null;
    #endregion

    #region Private Fields
    private bool _tildePressed = false;
    #endregion
    
    
    #region Private Fields
    private void Start()
    {
        
    }

    private void Update()
    {
        ReadInput();
        TrySwitchConsole();
    }

    private void ReadInput()
    {
        _tildePressed = Input.GetKeyDown(KeyCode.BackQuote);
    }

    private void TrySwitchConsole()
    {
        if (_tildePressed)
            console.gameObject.SetActiveInverse();
    }
    #endregion
}
