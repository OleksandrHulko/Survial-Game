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
    private Vector3Int[] availableRotations = null;
    [SerializeField]
    private Vector3Int[] rotations = new Vector3Int[6];
    #endregion

    #region Private Fields
    private Vector3 _raycastPosition = Vector3.zero;
    private Vector3 _normal = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;
    
    private int _rotationIndex = 1;
    #endregion
    

    #region Public Methods
    public void TrySetPositionAndRotation( Vector3 raycastPosition, Vector3 normal )
    {
        if(_normal != normal)
            SetRotation(normal);
        
        SetPosition(raycastPosition);
        
        _raycastPosition = raycastPosition;
        _normal = normal;

        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 objectPos = transform.position;

        Vector3 direction = objectPos - cameraPos;

        Debug.DrawRay(objectPos , direction.RoundToSide(), Color.red);
    }

    public void TryChangeRotation()
    {
        int index = _rotationIndex++ % availableRotations.Length;
        Debug.Log($"{index}");
        //transform.rotation = Quaternion.Euler(availableRotations[index]);

        //transform.rotation *= Quaternion.AngleAxis(90, transform.right);
        Vector3Int rotate = (transform.position - Camera.main.transform.position).RoundToSide();
        transform.Rotate(rotate, 90.0f, Space.World);
        if(Application.platform == RuntimePlatform.WebGLPlayer);
        //transform.Rotate(_normal, 90.0f);
    }

    public void Put()
    {
        foreach (Collider colliderElement in colliders)
        {
            colliderElement.enabled = true;
        }
    }
    #endregion
    
    #region Private Methods
    private void SetPosition(Vector3 raycastPosition)
    {
        Vector3Int roundToIntPosition = Vector3Int.RoundToInt(raycastPosition);
        float height = roundToIntPosition.y - raycastPosition.y;
        Vector3Int position = height <= 0.1f ? roundToIntPosition : roundToIntPosition - Vector3Int.up;

        if (transform.position != position)
        {
            transform.position = position;
            ResetRotation();
        }
    }
    
    private void SetRotation( Vector3 normal )
    {
        _rotation = Quaternion.Euler(GetRotation(normal));
        transform.rotation = _rotation;
    }

    private void ResetRotation()
    {
        transform.rotation = _rotation;
    }
    
    private Vector3Int GetRotation( Vector3 normal )
    {
        Vector3Int direction = Vector3Int.RoundToInt(normal);

        if (direction == Vector3Int.right)
            return rotations[0];
        if (direction == Vector3Int.left)
            return rotations[1];
        if (direction == Vector3Int.forward)
            return rotations[2];
        if (direction == Vector3Int.back)
            return rotations[3];
        if (direction == Vector3Int.up)
            return rotations[4];
        if (direction == Vector3Int.down)
            return rotations[5];

        return rotations[4];
    }
    #endregion
}
