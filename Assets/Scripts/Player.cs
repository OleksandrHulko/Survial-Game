using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class Player : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Ax ax = null;
    [SerializeField]
    private Camera cameraMain = null;
    [SerializeField]
    private PrefabsStorage prefabsStorage = null;
    #endregion

    #region Private Fields
    private static Player _instance = null;
    private static PlayerState _playerState = null;

    private bool _mouse0Pressed = false;

    private static FreePlayerState  _freePlayerState  = null;
    private static BusyPlayerState  _busyPlayerState  = null;
    private static BuildPlayerState _buildPlayerState = null;
    #endregion


    #region Private Methods
    private void Start()
    {
        _instance = this;

        _freePlayerState  = new FreePlayerState(cameraMain);
        _busyPlayerState  = new BusyPlayerState(cameraMain, ax);
        _buildPlayerState = new BuildPlayerState(cameraMain);
        
        prefabsStorage.Init(); // move to another class
        SetPlayerState(_busyPlayerState);
    }

    private void Update()
    {
        ReadInput();
        
        //if(_mouse0Pressed)
        //    _playerState.Mouse0Handler();
        
        _playerState.Update();
    }

    private void LateUpdate()
    {
        _playerState.LateUpdate();
    }

    private static void SetPlayerState(PlayerState playerState)
    {
        _playerState = playerState;
        _playerState.OnSetState();
    }

    private void ReadInput()
    {
        _mouse0Pressed = Input.GetKeyDown(KeyCode.Mouse0);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetPosition(), 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Vector3Int.RoundToInt(GetPosition()), 0.1f);
        
        Vector3 GetPosition()
        {
            Ray ray = cameraMain.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 10f))
            {
                return raycastHit.point;
            }
            return Vector3.zero;
        }
    }

    #endregion

    #region Public Methods
    public static Player GetInstance()
    {
        return _instance;
    }

    public static void SetPlayerState(PlayerStatE playerStatE)
    {
        switch (playerStatE)
        {
            case PlayerStatE.Free : SetPlayerState(_freePlayerState);
                break;
            case PlayerStatE.Busy : SetPlayerState(_busyPlayerState);
                break;
            case PlayerStatE.Build : SetPlayerState(_buildPlayerState);
                break;
        }
    }
    #endregion
}

public enum PlayerStatE : byte
{
    Free = 0 ,
    Busy     ,
    Build    ,
}

public abstract class PlayerState
{
    protected readonly Camera _playerCamera;
    protected readonly Vector3 _screenCenter = new Vector3(0.5f, 0.5f, 0);
    private const float RAY_DISTANCE = 3.0f;

    protected Animator playerAnimator = null;

    protected PlayerState(Camera playerCamera)
    {
        _playerCamera = playerCamera;
        
        playerAnimator = Player.GetInstance().GetComponent<Animator>();
    }

    protected abstract void SetLayerWeight();

    protected virtual void Mouse0Handler()
    {
        
    }

    protected virtual void Mouse1Handler()
    {
        
    }

    public virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
            Mouse0Handler();
        if(Input.GetKeyDown(KeyCode.Mouse1))
            Mouse1Handler();
    }

    public virtual void LateUpdate()
    {
        
    }
    
    public virtual void OnSetState()
    {
        SetLayerWeight();
    }

    protected void TryTakeObject( out bool relevant )
    {
        relevant = false;
        
        Ray ray = _playerCamera.ViewportPointToRay(_screenCenter);
        
        if (Physics.Raycast(ray, out RaycastHit raycastHit, RAY_DISTANCE))
        {
            if (raycastHit.collider.TryGetComponent(out ITake iTake))
            {
                iTake.Take();
                relevant = true;
            }
        }
    }
}

class FreePlayerState : PlayerState
{
    public FreePlayerState(Camera playerCamera) : base(playerCamera)
    {
    }

    protected override void Mouse0Handler()
    {
        TryTakeObject(out bool relevant);
    }

    protected override void SetLayerWeight()
    {
        playerAnimator.SetLayerWeight(1, 0.0f);
    }
}

class BusyPlayerState : PlayerState
{
    private IDamage _iDamage;

    public BusyPlayerState(Camera playerCamera, IDamage iDamage) : base(playerCamera)
    {
        _iDamage = iDamage;
    }

    protected override void Mouse0Handler()
    {
        TryTakeObject(out bool relevant);

        if (!relevant)
            _iDamage.Attack();
    }

    protected override void SetLayerWeight()
    {
        playerAnimator.SetLayerWeight(1, 1.0f);
    }
}

class BuildPlayerState : PlayerState
{
    private BuildingObject _buildingObject = null;
    
    public BuildPlayerState(Camera playerCamera) : base(playerCamera)
    {
    }

    protected override void Mouse0Handler()
    {
        _buildingObject.Put();
        
        if (Storage.CountOfObjectType(Inventory.selectedItem.objectType) > 0)
            InitBuildingObject();
        else
            Player.SetPlayerState(PlayerStatE.Free);
    }

    protected override void Mouse1Handler()
    {
        Object.Destroy(_buildingObject.gameObject);
        Player.SetPlayerState(PlayerStatE.Free);
    }
    
    public override void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
            _buildingObject.TryChangeRotation();

        (Vector3 position, Vector3 normal) positionAndNormal = GetRaycastPositionAndNormal();
        
        _buildingObject.TrySetPositionAndRotation(positionAndNormal.position , positionAndNormal.normal);
    }

    public override void OnSetState()
    {
        base.OnSetState();
        
        InitBuildingObject();
    }

    protected override void SetLayerWeight()
    {
        playerAnimator.SetLayerWeight(1, 0.0f);
    }

    private void InitBuildingObject()
    {
        _buildingObject = Object.Instantiate(PrefabsStorage.GetBuildingObject(Inventory.selectedItem.objectType));
    }
    
    private (Vector3 position, Vector3 normal) GetRaycastPositionAndNormal()
    {
        Ray ray = _playerCamera.ViewportPointToRay(_screenCenter);
        
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 10f))
        {
            return (raycastHit.point , raycastHit.normal.normalized);
        }

        return (Vector3.zero, Vector3.up);
    }
}