using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using OptionData = UnityEngine.UI.Dropdown.OptionData;

public class Settings : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private CanvasGroup canvasGroup = null;
    [SerializeField]
    private Text quitButtonText = null;
    
    [SerializeField]
    private Dropdown resolutionDropdown = null;
    [SerializeField]
    private Text resolutionText = null;
    
    [SerializeField]
    private Toggle vSyncToggle = null;
    [SerializeField]
    private Text vSyncText = null;
    
    [SerializeField]
    private Slider renderDistanceSlider = null;
    [SerializeField]
    private Text renderDistanceText = null;
    
    [SerializeField]
    private Slider chunkWidthSlider = null;
    [SerializeField]
    private Text chunkWidthSliderText = null;
    #endregion
    
    #region Private Fields
    private CanvasGroup _mainMenuCG = null;
    
    private static readonly string keyResolution     = "ResolutionDropdownValue";
    private static readonly string keyVSync          = "VSyncToggleValue";
    private static readonly string keyRenderDistance = "RenderDistanceSliderValue";
    private static readonly string keyChunkWidth     = "ChunkWidthSliderValue";
    #endregion

    #region Public Fields
    #endregion
    

    #region Public Methods
    public void Init(CanvasGroup mainMenuCG)
    {
        _mainMenuCG = mainMenuCG;

        quitButtonText.text = Localization.BACK;
        
        InitResolutionDropdownAndText();
        InitVSyncToggleAndText();
        InitRenderDistanceSliderAndText();
        InitChunkWidthSliderAndText();
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
    
    #region Private Methods
    private void InitResolutionDropdownAndText()
    {
        resolutionText.text = Localization.RESOLUTION;
        
        Resolution[] resolutions = Screen.resolutions.Where(x => x.refreshRate <= 60).Reverse().ToArray();

        foreach (Resolution resolution in resolutions)
            resolutionDropdown.options.Add(new OptionData($"{resolution.width}x{resolution.height}"));

        resolutionDropdown.onValueChanged.AddListener(ResolutionDropdownHandler);
        resolutionDropdown.value = PlayerPrefs.GetInt(keyResolution, 0);
        
        resolutionDropdown.RefreshShownValue();

        void ResolutionDropdownHandler( int value )
        {
            Screen.SetResolution(resolutions[value].width, resolutions[value].height, true);
            PlayerPrefs.SetInt(keyResolution, value);
        }
    }

    private void InitVSyncToggleAndText()
    {
        vSyncText.text = Localization.VSYNC;
        
        vSyncToggle.isOn = PlayerPrefs.GetInt(keyVSync, 1) == 1;
        vSyncToggle.onValueChanged.AddListener(VSyncToggleHandler);

        void VSyncToggleHandler(bool value)
        {
            QualitySettings.vSyncCount = value ? 1 : 0;
            PlayerPrefs.SetInt(keyVSync, value ? 1 : 0);
        }
    }

    private void InitRenderDistanceSliderAndText()
    {
        int renderDistance = PlayerPrefs.GetInt(keyRenderDistance, 10);
        
        renderDistanceText.text = string.Format(Localization.RENDER_DISTANCE, renderDistance);

        renderDistanceSlider.value = renderDistance;
        renderDistanceSlider.onValueChanged.AddListener(RenderDistanceSliderHandler);

        SettingsSaver.renderDistance = (byte)renderDistance;
        
        void RenderDistanceSliderHandler(float value)
        {
            renderDistance = Mathf.RoundToInt(value);
            PlayerPrefs.SetInt(keyRenderDistance, renderDistance);
            SettingsSaver.renderDistance = (byte)renderDistance;
            
            renderDistanceText.text = string.Format(Localization.RENDER_DISTANCE, renderDistance);
        }
    }

    private void InitChunkWidthSliderAndText()
    {
        int chunkWidthSavedValue = PlayerPrefs.GetInt(keyChunkWidth, 7);
        int chunkWidth = CalculateChunkWidth(chunkWidthSavedValue);

        chunkWidthSliderText.text = string.Format(Localization.CHUNK_WIDTH, chunkWidth);
        
        chunkWidthSlider.value = chunkWidthSavedValue;
        chunkWidthSlider.onValueChanged.AddListener(ChunkWidthSliderHandler);

        SettingsSaver.chunkWidth = (ushort)chunkWidth;

        void ChunkWidthSliderHandler(float value)
        {
            chunkWidth = CalculateChunkWidth(Mathf.RoundToInt(value));
            PlayerPrefs.SetInt(keyChunkWidth, Mathf.RoundToInt(value));
            SettingsSaver.chunkWidth = (ushort)chunkWidth;

            chunkWidthSliderText.text = string.Format(Localization.CHUNK_WIDTH, chunkWidth);
        }

        int CalculateChunkWidth( int savedValue )
        {
            return Mathf.RoundToInt(Mathf.Pow(2, savedValue));
        }
    }
    #endregion
}
