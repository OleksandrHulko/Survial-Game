using UnityEngine;
using UnityEngine.SceneManagement;
using Text = UnityEngine.UI.Text;

public class PausePanel : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Text title = null;
    [SerializeField]
    private Text leftButtonText = null;
    [SerializeField]
    private Text rightButtonText = null;
    #endregion

    #region Private Fields
    private bool _enabled = false;
    #endregion


    #region Public Methods
    public void Init()
    {
        title          .text = Localization.PAUSE;
        leftButtonText .text = Localization.QUIT_TO_MENU;
        rightButtonText.text = Localization.BACK;
    }

    public void ShowInverse()
    {
        _enabled = !_enabled;
        
        gameObject.SetActive(_enabled);
        
        Player    .GetInstance().enabled = !_enabled;
        Controller.GetInstance().enabled = !_enabled;
        
        Helper.SetVisibleCursor(_enabled);
        Time.timeScale = _enabled ? 0.0f : 1.0f;
    }

    public void LeftButtonHandler()
    {
        Player.SetPlayerState(PlayerStatE.Busy);
        SaveManager.GetInstance().SaveGameData();
        Inventory.selectedItem = (null, ObjectType.None);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
    
    public void RightButtonHandler()
    {
        ShowInverse();
    }
    #endregion
}
