using UnityEngine;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Text instructionText = null;
    [SerializeField]
    private Text quitButtonText = null;
    [SerializeField]
    private CanvasGroup canvasGroup = null;
    #endregion
    
    #region Private Fields
    private CanvasGroup _mainMenuCG = null;
    #endregion
    
    #region Public Fields
    #endregion
    
    
    #region Private Methods
    #endregion
    
    #region Public Methods
    public void Init(CanvasGroup mainMenuCG)
    {
        _mainMenuCG = mainMenuCG;
        
        instructionText.text = Localization.INSTRUCTION_INFO;
        quitButtonText .text = Localization.OK;
    }
    
    public void StopAllCoroutinesOverride()
    {
        StopAllCoroutines();
        canvasGroup.SetAlpha(0.0f);
    }

    public void Open()
    {
        StopAllCoroutines();

        StartCoroutine(_mainMenuCG.SmoothlySetAlpha(0.0f));
        StartCoroutine(canvasGroup.SmoothlySetAlpha(1.0f));
    }

    public void Quit()
    {
        StopAllCoroutines();
        
        StartCoroutine(_mainMenuCG.SmoothlySetAlpha(1.0f));
        StartCoroutine(canvasGroup.SmoothlySetAlpha(0.0f));
    }
    #endregion
}
