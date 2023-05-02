using UnityEngine;

class ThirdPersonCameraControl : CameraControl
{
    #region Constants
    private const float MAX_DELTA = 200f;
    private const float DISTANCE = 2.0f;
    private const float MIN_ROT_ANGLE = -80.0f;
    private const float MAX_ROT_ANGLE = 80.0f;
    #endregion

    
    #region Constructor
    public ThirdPersonCameraControl(Transform cameraTransform, Transform playerTransform) : base(cameraTransform, playerTransform) { }
    #endregion

    #region Override Methods
    public override void RotatePlayer(float mouseXAxis, bool isMoving)
    {
        if (isMoving)
            playerTransform.CorrectEulerAngles(y: Mathf.MoveTowardsAngle(playerTransform.rotation.eulerAngles.y, cameraTransform.rotation.eulerAngles.y, MAX_DELTA * Time.deltaTime), ySetMode: true);
    }

    public override void RotateCamera(float mouseXAxis, float mouseYAxis)
    {
        cameraTransform.CorrectEulerAngles((cameraTransform.rotation.eulerAngles.x - mouseYAxis).ClampedAngle(MIN_ROT_ANGLE, MAX_ROT_ANGLE), mouseXAxis, xSetMode: true);
        Vector3 destination = playerTransform.position + Vector3.up - (cameraTransform.forward * DISTANCE);
        cameraTransform.position = destination;
    }
    #endregion
}