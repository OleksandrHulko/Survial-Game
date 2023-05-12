using System;
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
    private static bool _tildePressed = false;
    private static bool _iPressed = false;

    private static bool _consoleEnabled = false;
    private static bool _inventoryEnabled = false;
    #endregion
    
    
    #region Private Fields
    private void Awake()
    {
        Helper.SetVisibleCursor(false);
    }

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
        {
            _consoleEnabled = !_consoleEnabled;
            console.gameObject.SetActive(_consoleEnabled);
        }
    }

    private void TrySwitchInventory()
    {
        if (_iPressed)
        {
            _inventoryEnabled = !_inventoryEnabled;
            
            SetEnabledPlayerControl(!_inventoryEnabled);
            inventory.gameObject.SetActive(_inventoryEnabled);
        }
    }

    private void SetEnabledPlayerControl( bool isEnabled )
    {
        Player    .GetInstance().enabled = isEnabled;
        Controller.GetInstance().enabled = isEnabled;
        Time.timeScale = isEnabled ? 1.0f : 0.0f;
    }
    #endregion

    #region Public Methods
    public static bool IsAnyWindowOpened()
    {
        return _consoleEnabled || _inventoryEnabled;
    }
    #endregion
}
