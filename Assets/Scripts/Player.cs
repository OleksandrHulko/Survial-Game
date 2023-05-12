using UnityEngine;

public class Player : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Ax ax = null;
    [SerializeField]
    private Camera cameraMain = null;
    #endregion

    #region Private Fields
    private static Player _instance = null;
    
    private const float RAY_DISTANCE = 3.0f;

    private bool _mouse0Pressed = false;
    private readonly Vector3 _screenCenter = new Vector3(0.5f, 0.5f, 0);
    #endregion


    #region Private Methods
    private void Start()
    {
        _instance = this;
    }

    private void Update()
    {
        ReadInput();
        
        if (!CanTakeObject())
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

    private bool CanTakeObject()
    {
        if (!_mouse0Pressed)
            return false;
        
        Ray ray = cameraMain.ViewportPointToRay(_screenCenter);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, RAY_DISTANCE))
        {
            if (raycastHit.collider.TryGetComponent(out ITake iTake))
            {
                iTake.Take();
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Public Methods
    public static Player GetInstance()
    {
        return _instance;
    }
    #endregion
}