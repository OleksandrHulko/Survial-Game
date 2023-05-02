using UnityEngine;

class FirstPersonCameraControl : CameraControl
{
    #region Private Fields
    private const float MIN_ROT_ANGLE = -80;
    private const float MAX_ROT_ANGLE = 80;
    
    private Transform _eyePosTransform = null;
    #endregion
    

    #region Constructor
    public FirstPersonCameraControl(Transform cameraTransform, Transform playerTransform, Transform eyePosTransform) : base(cameraTransform, playerTransform)
    {
        _eyePosTransform = eyePosTransform;
    }
    #endregion

    #region Override Methods
    public override void RotatePlayer(float mouseXAxis, bool isMoving)
    {
        playerTransform.Rotate(playerTransform.up, mouseXAxis);
    }

    public override void RotateCamera(float mouseXAxis, float mouseYAxis)
    {
       cameraTransform.CorrectEulerAngles(
            (cameraTransform.rotation.eulerAngles.x - mouseYAxis).ClampedAngle(MIN_ROT_ANGLE, MAX_ROT_ANGLE),
            playerTransform.rotation.eulerAngles.y,
            xSetMode: true, ySetMode: true);

       cameraTransform.position = _eyePosTransform.position;
    }
    #endregion
}
