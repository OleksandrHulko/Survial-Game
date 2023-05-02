using System;
using UnityEngine;

public class PlayerAnimationEventsManager : MonoBehaviour
{
    #region Actions
    public static Action OnHoldingStart  = null;
    public static Action OnFellingStart  = null;
    public static Action OnFellingDamage = null;
    public static Action OnFellingEnd    = null;
    #endregion

    
    #region Private Methods
    private void HoldingStartHandler()
    {
        OnHoldingStart?.Invoke();
    }
    
    private void FellingStartHandler()
    {
        OnFellingStart?.Invoke();
    }

    private void FellingDamageHandler()
    {
        OnFellingDamage?.Invoke();
    }

    private void FellingEndHandler()
    {
        OnFellingEnd?.Invoke();
    }
    #endregion
}
