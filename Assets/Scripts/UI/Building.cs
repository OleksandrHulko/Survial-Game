using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Transform arrowsT = null;
    [SerializeField]
    private Transform cameraT = null;
    [SerializeField]
    private Text rotationInfoText = null;
    [SerializeField]
    private Text mouseInfoText = null;
    [SerializeField]
    private CanvasGroup mouseInfoCG = null;
    #endregion

    #region Private Fields
    private static Building _instance = null;
    private readonly int _secondsToShowMouseInfo = 5;
    #endregion


    #region Private Methods
    private void Awake()
    {
        _instance = this;
        SetRotationInfoText();
        SetMouseInfoText();
    }

    private void OnEnable()
    {
        arrowsT.gameObject.SetActive(true);
        mouseInfoCG.SetAlpha(1.0f, true);
        StartCoroutine(HideMouseInfoText());
    }

    private void OnDisable()
    {
        arrowsT.gameObject.SetActive(false);
        StopAllCoroutines();
    }
    #endregion

    #region Public Methods
    public void SetTransformForArrows( Vector3 objectPosition, Vector3 forward, Vector3 upwards )
    {
        arrowsT.position = objectPosition;
        arrowsT.rotation = Quaternion.LookRotation(forward, upwards);
        arrowsT.localScale = Vector3.one * (GetDistance() / 5.0f);
        
        float GetDistance()
        {
            return Vector3.Distance(cameraT.position, objectPosition);
        }
    }

    public void ShowArrows(bool show)
    {
        arrowsT.gameObject.SetActive(show);
        rotationInfoText.enabled = show;
    }

    public static Building GetInstance()
    {
        return _instance;
    }
    #endregion

    #region Private Methods
    private void SetRotationInfoText()
    {
        rotationInfoText.text = Localization.BUILDING_INFO_ROTATION;
    }

    private void SetMouseInfoText()
    {
        mouseInfoText.text = Localization.BUILDING_INFO_MOUSE;
    }

    private IEnumerator HideMouseInfoText()
    {
        yield return new WaitForSeconds(_secondsToShowMouseInfo);
        yield return mouseInfoCG.SmoothlySetAlpha(0.0f, notBlockingRaycast: true);
    }
    #endregion
}
