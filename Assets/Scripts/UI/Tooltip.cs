using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Text text = null;
    [SerializeField]
    private CanvasGroup canvasGroup = null;
    #endregion

    #region Private Fields
    private static Tooltip _instance = null;
    
    private string _message = string.Empty;
    private float _seconds = 0.0f;
    #endregion
    

    #region Private Methods
    private void Awake()
    {
        _instance = this;
    }

    private void OnDisable()
    {
        if (canvasGroup.alpha == 0)
            return;
        
        StopAllCoroutines();
        canvasGroup.SetAlpha(0.0f);
    }

    private void SetText()
    {
        text.text = _message;
    }

    private IEnumerator SetAlpha()
    {
        yield return canvasGroup.SmoothlySetAlpha(1.0f, 0.2f);
        yield return new WaitForSecondsRealtime(_seconds);
        yield return canvasGroup.SmoothlySetAlpha(0.0f, 0.2f);
    }
    #endregion
    
    #region Public Methods
    public static void InitStatic(string message, float seconds = 0.5f)
    {
        _instance.Init(message, seconds);
    }
    
    public void Init(string message, float seconds = 0.5f)
    {
        _message = message;
        _seconds = seconds;
        
        SetText();

        StopAllCoroutines();
        StartCoroutine(SetAlpha());
    }
    #endregion
}
