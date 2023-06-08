using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Text playButtonText = null;
    [SerializeField]
    private Text settingsButtonText = null;
    [SerializeField]
    private Text instructionButtonText = null;
    [SerializeField]
    private Text quitButtonText = null;
    [SerializeField]
    private Instruction instruction = null;
    [SerializeField]
    private Settings settings = null;
    [SerializeField]
    private CanvasGroup canvasGroup = null;
    #endregion
    
    #region Private Fields
    private const string GITHUB_URL = "https://github.com/OleksandrHulko/Survial-Game";
    #endregion
    
    #region Public Fields
    #endregion
    
    
    #region Private Methods
    private void Awake()
    {
        SetButtonTexts();
        instruction.Init(canvasGroup);
        settings.Init(canvasGroup);
    }

    private void SetButtonTexts()
    {
        playButtonText       .text = Localization.PLAY;
        settingsButtonText   .text = Localization.SETTINGS;
        instructionButtonText.text = Localization.INSTRUCTION;
        quitButtonText       .text = Localization.QUIT;
    }
    #endregion
    
    #region Public Methods
    public void OpenGitHub()
    {
        Application.OpenURL(GITHUB_URL);
    }

    public void Play()
    {
        canvasGroup.SetAlpha(0.0f);
        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        instruction.StopAllCoroutinesOverride();
        settings.Open();
    }

    public void OpenInstruction()
    {
        settings.StopAllCoroutinesOverride();
        instruction.Open();
    }

    public void Quit()
    {
        StartCoroutine(QuitSmoothly());

        IEnumerator QuitSmoothly()
        {
            yield return canvasGroup.SmoothlySetAlpha(0.0f);
            Application.Quit();
        }
    }
    #endregion
}
