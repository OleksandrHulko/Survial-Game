using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Console console = null;
    [SerializeField]
    private Inventory inventory = null;
    [SerializeField]
    private Building building = null;
    [SerializeField]
    private PausePanel pausePanel = null;
    #endregion

    #region Private Fields
    private static UI _instance = null;
    
    private static bool _tildePressed = false;
    private static bool _iPressed = false;
    private static bool _escapePressed = false;

    private static bool _consoleEnabled = false;
    private static bool _inventoryEnabled = false;
    #endregion

    #region Public Fields
    #endregion
    
    
    #region Private Fields
    private void Awake()
    {
        _instance = this;
        
        pausePanel.Init();
        
        Helper.SetVisibleCursor(false);
    }

    private void Update()
    {
        ReadInput();
        TrySwitchConsole();
        TrySwitchInventory();
        TrySwitchPause();
    }

    private void ReadInput()
    {
        _tildePressed  = Input.GetKeyDown(KeyCode.BackQuote);
        _iPressed      = Input.GetKeyDown(KeyCode.I);
        _escapePressed = Input.GetKeyDown(KeyCode.Escape);
    }

    private void TrySwitchConsole()
    {
        if (_tildePressed)
        {
            if (pausePanel.gameObject.activeSelf)
                return;
            
            _consoleEnabled = !_consoleEnabled;
            console.gameObject.SetActive(_consoleEnabled);
        }
    }

    private void TrySwitchInventory()
    {
        if (_iPressed)
        {
            if (pausePanel.gameObject.activeSelf)
                return;
            
            _inventoryEnabled = !_inventoryEnabled;
            
            SetEnabledPlayerControl(!_inventoryEnabled);
            inventory.gameObject.SetActive(_inventoryEnabled);
        }
    }

    private void TrySwitchPause()
    {
        if (_escapePressed && !IsAnyWindowOpened())
            pausePanel.ShowInverse();
    }

    private void SetEnabledPlayerControl( bool isEnabled )
    {
        Player    .GetInstance().enabled = isEnabled;
        Controller.GetInstance().enabled = isEnabled;
        Time.timeScale = isEnabled ? 1.0f : 0.0f;
    }

    private void SetActiveBuildingNonStatic(bool active)
    {
        building.gameObject.SetActive(active);
    }
    #endregion

    #region Public Methods
    public static bool IsAnyWindowOpened()
    {
        return _consoleEnabled || _inventoryEnabled;
    }

    public static void SetActiveBuilding(bool active)
    {
        _instance.SetActiveBuildingNonStatic(active);
    }
    #endregion
}
