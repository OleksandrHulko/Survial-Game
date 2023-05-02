using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Transform cameraTransform = null;
    [SerializeField]
    private Transform eyePosTransform = null;
    [SerializeField]
    private CharacterController characterController = null;
    [SerializeField]
    private Animator animator = null;
    #endregion

    #region Private Fields
    private static Controller _instance = null;
    
    private static readonly int _x    = Animator.StringToHash("X");
    private static readonly int _y    = Animator.StringToHash("Y");
    private static readonly int _jump = Animator.StringToHash("Jump");

    private const float WALK_SPEED = 1.5f;
    private const float RUN_SPEED = 60.0f;
    private const float JUMP_SPEED = 3.0f;
    private const float G = -9.81f;

    private CameraControl _cameraControl = null;
    private FirstPersonCameraControl _firstPersonCameraControl = null;
    private ThirdPersonCameraControl _thirdPersonCameraControl = null;

    private bool _spacePressed = false;
    private bool _vPressed = false;
    private bool _isGrounded = false;
    private bool _isMoving = false;
    #endregion

    #region Private Fields
    private Vector3 _speed = Vector3.zero;
    private Vector2 _axis  = Vector2.zero;

    private float _verticalSpeed = 0.0f;

    private float _horizontalAxis = 0.0f;
    private float _verticalAxis = 0.0f;
    private float _mouseXAxis = 0.0f;
    private float _mouseYAxis = 0.0f;
    private float _accelerationAxis = 0.0f;
    #endregion


    #region Private Methods
    private void Start()
    {
        _instance = this;
        SetSavedHeight();
        SetSavedRotation();
        InitCameraControl();
        SetStrategyCameraControl(_firstPersonCameraControl);
        ResetPositionManager.AddTransform(transform);
        ResetPositionManager.AddTransform(cameraTransform);
    }

    private void Update()
    {
        ReadInput();
        CalculateVerticalSpeed();
        Move();
        TryChangeCameraControl();
        RotatePlayer();
        RotateCamera();
        ControlAnimations();
    }

    private void ReadInput()
    {
        _horizontalAxis   = Input.GetAxis("Horizontal");
        _verticalAxis     = Input.GetAxis("Vertical");
        _mouseXAxis       = Input.GetAxis("Mouse X");
        _mouseYAxis       = Input.GetAxis("Mouse Y");
        _accelerationAxis = Input.GetAxis("Acceleration");

        _spacePressed = Input.GetKeyDown(KeyCode.Space);
        _vPressed     = Input.GetKeyDown(KeyCode.V);
    }

    private void Move()
    {
        _isMoving = _horizontalAxis != 0.0f || _verticalAxis != 0.0f;
            
        _axis = new Vector2(_horizontalAxis, _verticalAxis);

        if (_axis.sqrMagnitude > 1)
            _axis.Normalize();

        _speed = new Vector3(_axis.x * GetNeededMoveSpeed(), _verticalSpeed, _axis.y * GetNeededMoveSpeed());

        characterController.Move(transform.TransformDirection(_speed) * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        _cameraControl.RotatePlayer(_mouseXAxis, _isMoving);
    }

    private void RotateCamera()
    {
        _cameraControl.RotateCamera(_mouseXAxis, _mouseYAxis);
    }

    private void CalculateVerticalSpeed()
    {
        _isGrounded = characterController.isGrounded;

        if (_isGrounded)
            _verticalSpeed = _spacePressed ? JUMP_SPEED : -1.0f;
        else
            _verticalSpeed += G * Time.deltaTime;
    }

    private void ControlAnimations()
    {
        if ( _spacePressed && _isGrounded )
            animator.SetTrigger(_jump);

        animator.SetFloat(_x, _speed.x);
        animator.SetFloat(_y, _speed.z);
    }

    private void SetStrategyCameraControl( CameraControl cameraControl )
    {
        _cameraControl = cameraControl;
    }

    private void InitCameraControl()
    {
        _firstPersonCameraControl = new FirstPersonCameraControl(cameraTransform, transform, eyePosTransform);
        _thirdPersonCameraControl = new ThirdPersonCameraControl(cameraTransform, transform);
    }

    private void TryChangeCameraControl()
    {
        if (!_vPressed)
            return;

        if (_cameraControl == _thirdPersonCameraControl)
            SetStrategyCameraControl(_firstPersonCameraControl);
        else
            SetStrategyCameraControl(_thirdPersonCameraControl);
    }

    private void SetSavedRotation()
    {
        transform.CorrectEulerAngles(y: SaveManager.GetInstance().playerYEulerAngle, ySetMode: true);
    }

    private void SetSavedHeight()
    {
        transform.SetYPosition(SaveManager.GetInstance().playerPosition?.y ?? LandscapeGenerator.GetInstance().GetSurfaceHeight(transform.position.ToVector2Int()));
    }

    private float GetNeededMoveSpeed()
    {
        if (_accelerationAxis == 0.0f)
            return WALK_SPEED;

        float runSpeed = RUN_SPEED * _accelerationAxis;

        return runSpeed > WALK_SPEED ? runSpeed : WALK_SPEED;
    }
    #endregion

    #region Public Methods
    public static Controller GetInstance()
    {
        return _instance;
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
    #endregion
}
