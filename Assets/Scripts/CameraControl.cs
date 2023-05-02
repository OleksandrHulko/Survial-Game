using UnityEngine;

public abstract class CameraControl
{
    #region Protected Fields
    protected Transform cameraTransform;
    protected Transform playerTransform;
    #endregion

    
    #region Constructor
    protected CameraControl( Transform cameraTransform, Transform playerTransform )
    {
        this.cameraTransform = cameraTransform;
        this.playerTransform = playerTransform;
    }
    #endregion

    #region Abstract Methods
    public abstract void RotatePlayer(float mouseXAxis , bool isMoving);
    
    public abstract void RotateCamera(float mouseXAxis, float mouseYAxis);
    #endregion
}
