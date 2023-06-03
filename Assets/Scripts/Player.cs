using System;
using System.Collections.Generic;
using System.Linq;
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
    
    protected virtual void Mouse2Handler()
    {
        
    }

    public virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
            Mouse0Handler();
        if(Input.GetKeyDown(KeyCode.Mouse1))
            Mouse1Handler();
        if(Input.GetKeyDown(KeyCode.Mouse2))
            Mouse2Handler();
    }

    public virtual void LateUpdate()
    {
        
    }
    
    public virtual void OnSetState()
    {
        Type type = GetType();

        if (type == typeof(FreePlayerState) || type == typeof(BusyPlayerState))
            UI.SetActiveBuilding(false);
        
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
    private ObjectType _objectType = ObjectType.None;
    private bool _isInventoryOpen = false;
    private const int RAY_DISTANCE = 10;

    public BuildPlayerState(Camera playerCamera) : base(playerCamera)
    {
        Inventory.OnInventoryOpen += OnInventoryOpenHandler;
        Inventory.OnInventoryClosed += OnInventoryClosedHandler;
    }

    protected override void Mouse0Handler()
    {
        if (!_buildingObject || !_buildingObject.CanPut())
            return;

        if (Storage.CountOfObjectType(_objectType) > 0)
            InitBuildingObject();
        else
        {
            Player.SetPlayerState(PlayerStatE.Busy);
            _buildingObject = null;
        }
    }

    protected override void Mouse1Handler()
    {
        TryDeleteCurrentBuildingObject();
    }

    protected override void Mouse2Handler()
    {
        TryDeleteBuildingObject();

        TryDeleteCurrentBuildingObject();
    }
    
    public override void LateUpdate()
    {
        if (_isInventoryOpen || !_buildingObject)
            return;

        _buildingObject.TrySetPositionAndRotation(GetRaycastHit(), _playerCamera.transform.position);

        _buildingObject.TryChangeRotation(
              Input.GetKeyDown(KeyCode.Alpha1)
            , Input.GetKeyDown(KeyCode.Alpha2)
            , Input.GetKeyDown(KeyCode.Alpha3)
        );
    }

    protected override void SetLayerWeight()
    {
        playerAnimator.SetLayerWeight(1, 0.0f);
    }

    private void InitBuildingObject()
    {
        _buildingObject = Object.Instantiate(PrefabsStorage.GetBuildingObject(_objectType), Vector3.down, Quaternion.identity);
        _buildingObject.Init();
    }

    private void OnInventoryOpenHandler()
    {
        _isInventoryOpen = true;
        
        UI.SetActiveBuilding(false);
        
        if (_buildingObject)
            _buildingObject.Delete();
    }

    private void OnInventoryClosedHandler( ObjectType objectType )
    {
        _isInventoryOpen = false;
        
        _objectType = objectType;

        if (_objectType == ObjectType.None)
            Player.SetPlayerState(PlayerStatE.Busy);

        if (_objectType.CanUseForBuild())// redudant
        {
            UI.SetActiveBuilding(true);
            InitBuildingObject();
        }
    }
    
    private RaycastHit GetRaycastHit()
    {
        Ray ray = _playerCamera.ViewportPointToRay(_screenCenter);
        Physics.Raycast(ray, out RaycastHit raycastHit, RAY_DISTANCE);

        return raycastHit;
    }

    private void TryDeleteBuildingObject()
    {
        Ray ray = _playerCamera.ViewportPointToRay(_screenCenter);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, RAY_DISTANCE))
        {
            if (raycastHit.collider.TryGetComponent(out BuildingObject buildingObject))
                buildingObject.DeletePutted();
        }
    }

    private void TryDeleteCurrentBuildingObject()
    {
        if (_buildingObject)
        {
            _buildingObject.Delete();
            Inventory.selectedItem = (null, ObjectType.None);
        }
    }
}