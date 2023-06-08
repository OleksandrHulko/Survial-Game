using System;
using UnityEngine;
using RenderSettings = UnityEngine.RenderSettings;

public class TimeManager : MonoBehaviour
{
    #region Serialize Fields
    [Header("Lenght of day in real seconds")]
    [SerializeField] 
    private int lengthOfDay = 0;
    [Range(0,1)]
    [SerializeField]
    private float dayCycle = 0;
    [SerializeField]
    private Light sun = null;
    [SerializeField]
    private AnimationCurve sunIntensity = null;
    [SerializeField]
    private Material daySkybox = null;
    [SerializeField]
    private Material nightSkybox = null;

    #endregion

    #region Private Fields
    private static readonly int _exposureId = Shader.PropertyToID("_Exposure");
    private static readonly int _xId = Shader.PropertyToID("_RotationX");
    private static readonly int _yId = Shader.PropertyToID("_RotationY");
    private static readonly int _zId = Shader.PropertyToID("_RotationZ");

    private static float _enableNightSkybox = 0.585f;
    private static float _disableNightSkybox = 0.915f;
    private static float _fadingTime = 0.1f;

    private static TimeManager _instance = null;
    //private TimeSpan _gameTime = TimeSpan.Zero;
    private static int _days = 0;
    private bool _usedNightSkybox = false;
    #endregion


    #region Private Methods
    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        SetSavedTimeValues();// TODO init values from SaveManager
    }

    private void Update()
    {
        IncrementDayCycle();
        ControlSky();
    }

    private void ControlSky()
    {
        TrySetSkybox();
        
        float intensity = sunIntensity.Evaluate(dayCycle);

        if (_usedNightSkybox)
        {
            RotateNightSkyBox();
            nightSkybox.SetFloat(_exposureId, GetNightSkyboxExposure());
        }
        else
        {
            sun.transform.localRotation = Quaternion.Euler(0, dayCycle * 360.0f, 0);
            sun.intensity = intensity;
        }

        RenderSettings.ambientIntensity = intensity;
        RenderSettings.reflectionIntensity = intensity;
    }

    private void IncrementDayCycle()
    {
        dayCycle += Time.deltaTime / (lengthOfDay);
        
        if (dayCycle > 1)
        {
            dayCycle = 0;
            _days++;
        }
    }

    private void RotateNightSkyBox()
    {
        Vector3 start = new Vector3(-16, 83, 0);
        Vector3 end = new Vector3(-82, -25, -65);

        Vector3 eulerAngles = Vector3.Lerp(start, end, Mathf.InverseLerp(_enableNightSkybox, _disableNightSkybox, dayCycle));
        
        nightSkybox.SetFloat(_xId,eulerAngles.x);
        nightSkybox.SetFloat(_yId,eulerAngles.y);
        nightSkybox.SetFloat(_zId,eulerAngles.z);
    }

    private void TrySetSkybox()
    {
        bool needToUseNightSkybox = dayCycle >= _enableNightSkybox && dayCycle <= _disableNightSkybox;

        if (needToUseNightSkybox && !_usedNightSkybox)
        {
            RenderSettings.skybox = nightSkybox;
            _usedNightSkybox = true;
        }
        else if ( !needToUseNightSkybox && _usedNightSkybox)
        {
            RenderSettings.skybox = daySkybox;
            _usedNightSkybox = false;
        }
    }
    
    private void SetSavedTimeValues()
    {
        _days = SaveManager.GetInstance().days;
        dayCycle = SaveManager.GetInstance().dayCycle;
    }

    private TimeSpan GetTimeFromFirstDay()
    {
        float hours = dayCycle * 24;
        float minutes = (hours % 1) * 60;
        float seconds = (minutes % 1) * 60;
        
        return new TimeSpan(_days, (int) hours, (int) minutes, (int) seconds);
    }

    private float GetNightSkyboxExposure()
    {
        if (dayCycle >= _enableNightSkybox && dayCycle <= _enableNightSkybox + _fadingTime)
            return Mathf.InverseLerp(_enableNightSkybox, _enableNightSkybox + _fadingTime, dayCycle);
        
        if (dayCycle >= _disableNightSkybox - _fadingTime && dayCycle <= _disableNightSkybox)
            return Mathf.InverseLerp(_disableNightSkybox, _disableNightSkybox - _fadingTime, dayCycle);
        
        return 1.0f;
    }
    #endregion

    #region Public Methods
    public static TimeManager GetInstance()
    {
        return _instance;
    }

    public (int days, float dayCycle) GetTimeValues()
    {
        return (_days, dayCycle);
    }

    public void AddHours(int hours = 1)
    {
        dayCycle += hours * (1 / 24f);
        if (dayCycle < 0)
        {
            dayCycle = 1 + dayCycle;
            _days--;
        }
    }
    #endregion
}
