using UnityEngine;

public class UI : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Console console = null;
    [SerializeField]
    private Inventory inventory = null;
    #endregion

    #region Private Fields
    private bool _tildePressed = false;
    private bool _iPressed = false;
    #endregion
    
    
    #region Private Fields
    private void Update()
    {
        ReadInput();
        TrySwitchConsole();
        TrySwitchInventory();
    }

    private void ReadInput()
    {
        _tildePressed = Input.GetKeyDown(KeyCode.BackQuote);
        _iPressed = Input.GetKeyDown(KeyCode.I);
    }

    private void TrySwitchConsole()
    {
        if (_tildePressed)
            console.gameObject.SetActiveInverse();
    }

    private void TrySwitchInventory()
    {
        if(_iPressed)
            inventory.gameObject.SetActiveInverse();
    }
    #endregion
}
