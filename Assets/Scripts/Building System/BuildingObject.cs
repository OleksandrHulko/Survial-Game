using System;
using UnityEngine;

public class BuildingObject : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private ObjectType objectType = ObjectType.None;
    [SerializeField]
    private Collider[] colliders = null;
    [SerializeField]
    private Vector3Int[] rotations = new Vector3Int[6];
    [SerializeField]
    private Vector3 overlapHalfExtents = Vector3.zero;
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;
    [SerializeField]
    private new Renderer renderer = null;
    [SerializeField]
    private Material normalMaterial = null;
    [SerializeField]
    private Material redMaterial = null;
    #endregion

    #region Private Fields
    private Vector3 _raycastPosition = Vector3.zero;
    private Vector3 _normal          = Vector3.zero;
    private Vector3 _cameraPosition  = Vector3.zero;
    private Vector3 _direction       = Vector3.zero;
    private Vector3 _perpendicular1  = Vector3.zero;
    private Vector3 _perpendicular2  = Vector3.zero;
    private Vector3 _emptyPosition   = new Vector3(0, -10, 0);
    
    private Quaternion _rotation        = Quaternion.identity;
    private Collider[] _collidersResult = new Collider[4];
    private Building _building = null;
    private Transform _raycastTransform = null;
    
    private int  _layer           = 0;
    private bool _changeRotation = false;
    private bool _canPlace       = false;
    #endregion

    #region Public Fields
    public ObjectType ObjectType => objectType;
    #endregion
    

    #region Public Methods
    public void Init()
    {
        _building = Building.GetInstance();
        _building.ShowArrows(!objectType.IsFoundation());
        EnabledColliders(false);
    }
    
    public void TrySetPositionAndRotation( RaycastHit raycastHit, Vector3 cameraPosition )
    {
        if (IsRaycastHitEmpty(raycastHit))
            return;
        
        _raycastPosition  = raycastHit.point;
        _normal           = raycastHit.normal.normalized;
        _layer            = raycastHit.collider.gameObject.layer;
        _raycastTransform = raycastHit.transform;
        _cameraPosition   = cameraPosition;

        SetPosition(_raycastPosition);
        CalculatePerpendiculars();
        SetRotation(_normal);

        _canPlace = CanPlace();
        
        SetNeededMaterial();

        _building.SetTransformForArrows(transform.position, _perpendicular1, _direction);// shit
    }

    public void TryChangeRotation( bool alpha1, bool alpha2, bool alpha3 )
    {
        if (objectType.IsFoundation())
            return;
        
        bool anyKeyWasPressed = alpha1 || alpha2 || alpha3;

        if (!anyKeyWasPressed)
            return;
        
        _changeRotation = true;
        
        if (alpha1)
            transform.Rotate(_direction, -90.0f, Space.World);
        else if (alpha2)
            transform.Rotate(_perpendicular1, -90.0f, Space.World);
        else
            transform.Rotate(_perpendicular2, -90.0f, Space.World);
    }

    public bool CanPut()
    {
        if (!_canPlace)
            return false;
        
        EnabledColliders();

        _building.ShowArrows(false);

        BuildingObjectsSpawner.Add((BuildingObjectInfo)this);
        Storage.TryRemove(objectType, 1, out bool successfully);
        
        Chunk chunk = _raycastTransform.GetComponentInParent<Chunk>();

        chunk.AddNewBuildingObjects(this);
        transform.SetParent(chunk.transform);

        return true;
    }

    public void Delete()
    {
        _building.ShowArrows(false);
        Destroy();
    }

    public void DeletePutted()
    {
        BuildingObjectsSpawner.Remove((BuildingObjectInfo)this);
        Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    #endregion
    
    #region Private Methods
    private void CalculatePerpendiculars()
    {
        _direction = (transform.position - _cameraPosition).RoundToSide();

        if (_direction == Vector3.up || _direction == Vector3.down)
        {
            _perpendicular1 = Vector3.Cross(_direction, Vector3Int.right);
            _perpendicular2 = Vector3.Cross(_direction, _perpendicular1);
        }
        else
        {
            _perpendicular1 = Vector3.Cross(_direction, Vector3.up);
            _perpendicular2 = Vector3.Cross(_direction, _perpendicular1);
        }
        
        //Debug.DrawRay(transform.position, _direction,      Color.red);
        //Debug.DrawRay(transform.position, _perpendicular1, Color.green);
        //Debug.DrawRay(transform.position, _perpendicular2, Color.blue);
    }
    
    private void SetPosition(Vector3 raycastPosition)
    {
        Vector3Int position = GetPosition(raycastPosition);

        if (transform.position != position)
        {
            transform.position = position;
            ResetRotation();
            _changeRotation = false;
        }
    }
    
    private void SetRotation( Vector3 normal )
    {
        if (objectType.IsFoundation())
            return;

        if (_changeRotation)
            return;
        
        normal = normal.RoundToSide();
        
        bool directionIsNotDownOrUp = _direction != Vector3.down && _direction != Vector3.up;
        bool normalIsDownOrUp = normal == Vector3.down || normal == Vector3.up;
        
        _rotation = Quaternion.Euler(GetRotation(normal));

        if (directionIsNotDownOrUp && normalIsDownOrUp)
            _rotation *= Quaternion.LookRotation(_direction);

        transform.rotation = _rotation;
    }

    private void ResetRotation()
    {
        transform.rotation = _rotation;
    }

    private void SetNeededMaterial()
    {
        renderer.material = _canPlace ? normalMaterial : redMaterial;
    }

    private void EnabledColliders(bool enabled = true)
    {
        foreach (Collider colliderElement in colliders)
            colliderElement.enabled = enabled;
    }
    
    private Vector3Int GetRotation( Vector3 normal )
    {
        if (normal == Vector3Int.right)
            return rotations[0];
        if (normal == Vector3Int.left)
            return rotations[1];
        if (normal == Vector3Int.forward)
            return rotations[2];
        if (normal == Vector3Int.back)
            return rotations[3];
        if (normal == Vector3Int.up)
            return rotations[4];
        if (normal == Vector3Int.down)
            return rotations[5];

        return rotations[4];
    }

    private Vector3Int GetPosition(Vector3 raycastPosition)
    {
        Vector3Int roundToIntPosition = Vector3Int.RoundToInt(raycastPosition);

        if (objectType.IsFoundation())
        {
            float height = roundToIntPosition.y - raycastPosition.y;
            return height <= 0.1f ? roundToIntPosition : roundToIntPosition - Vector3Int.up;
        }

        return roundToIntPosition;
    }

    private bool CanPlace()
    {
        bool isFoundation = objectType.IsFoundation();
        int layerMask = isFoundation ? ~(1 << Helper.SurfaceLayer) : -1;
        
        int collidersCount = Physics.OverlapBoxNonAlloc(
              transform.position + transform.TransformDirection(centerOffset)
            , overlapHalfExtents
            , _collidersResult
            , transform.rotation,
              layerMask
        );

        if (collidersCount > 0)
            return false;

        if (BuildingObjectsSpawner.Overlap((BuildingObjectInfo)this))
            return false;
        
        if (isFoundation)
            return _layer.IsSurfaceLayer() || _layer.IsFoundationLayer();

        return _layer.IsFoundationLayer() || _layer.IsBuildingObjectLayer();
    }

    private bool IsRaycastHitEmpty(RaycastHit raycastHit)
    {
        bool empty = !raycastHit.transform;

        if (empty)
        {
            _building.ShowArrows(false);
            
            transform.position = _emptyPosition;
        }
        else
            _building.ShowArrows(!objectType.IsFoundation());

        return empty;
    }
    #endregion
}
