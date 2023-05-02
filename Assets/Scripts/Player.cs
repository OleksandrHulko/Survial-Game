using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Ax ax = null;
    #endregion

    #region Private Fields
    private static Player _instance = null;

    private bool _mouse0Pressed = false;
    #endregion


    #region Private Methods
    private void Start()
    {
        _instance = this;
    }

    private void Update()
    {
        ReadInput();
        TryAttack();
    }

    private void ReadInput()
    {
        _mouse0Pressed = Input.GetKeyDown(KeyCode.Mouse0);
    }

    private void TryAttack()
    {
        if (!_mouse0Pressed)
            return;
        
        ax.Attack();
    }
    #endregion

    #region Public Methods
    public static Player GetInstance()
    {
        return _instance;
    }
    #endregion
}