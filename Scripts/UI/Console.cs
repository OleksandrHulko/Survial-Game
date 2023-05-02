using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Transform heroTransform = null;
    [SerializeField]
    private Text virtualPositionText = null;
    [SerializeField]
    private Text realPositionText = null;
    [SerializeField]
    private Text timePlayingText = null;
    [SerializeField]
    private Text fpsText = null;
    [SerializeField]
    private Text felledTreesCountText = null;
    #endregion

    #region Private Fields
    private readonly string virtualPositionTxt = "Player position - X: {0}, Y: {1}, Z: {2}";
    private readonly string realPositionTxt = "Player position - X: {0}, Y: {1}, Z: {2} (In Unity Scene)";
    private readonly string timePlayingTxt = "Time playing: {0} hours {1} minutes {2} seconds";
    private readonly string fpsTxt = "FPS: {0}";
    private readonly string felledTreesCountTxt = "Felled trees count - {0}";

    private const float SECONDS_TO_UPDATE_FPS_TXT = 0.1f;
    #endregion
    
    
    #region Private Methods
    private void OnEnable()
    {
        StartCoroutine(UpdateFpsText());
        StartCoroutine(UpdateFelledTreesCountText());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        SetVirtualPositionText();
        SetRealPositionText();
        SetTimePlayingText();
    }

    private IEnumerator UpdateFpsText()
    {
        while (true)
        {
            SetFpsText();
            yield return new WaitForSeconds(SECONDS_TO_UPDATE_FPS_TXT);
        }
    }

    private void SetVirtualPositionText()
    {
        Vector3 position = heroTransform.position.ToVirtualPosition();
        virtualPositionText.text = string.Format(virtualPositionTxt, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
    }

    private void SetRealPositionText()
    {
        Vector3 position = heroTransform.position;// Vector3Int ???
        realPositionText.text = string.Format(realPositionTxt, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
    }

    private void SetTimePlayingText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time);
        timePlayingText.text = string.Format(timePlayingTxt, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
    
    private void SetFpsText()
    {
        fpsText.text = string.Format(fpsTxt, GetFps());
    }

    private void SetFelledTreesCountText()
    {
        felledTreesCountText.text = string.Format(felledTreesCountTxt, TreeSpawner.felledTreesPositions.Count);
    }

    private IEnumerator UpdateFelledTreesCountText()
    {
        while (true)
        {
            SetFelledTreesCountText();
            yield return new WaitForSeconds(1.0f);
        }
    }

    private float GetFps()
    {
        return Mathf.RoundToInt(1.0f / Time.smoothDeltaTime);
    }
    #endregion

    #region Public Methods

    public void RestoreFelledTrees()
    {
        TreeSpawner.felledTreesPositions.Clear();
    }
    #endregion
}
