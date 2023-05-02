using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionManager : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Transform playerTransform = null;
    #endregion

    #region Private Fields
    private static List<Transform> _transforms = new List<Transform>();// transforms that need to be moved closer to the center of coordinates
    
    private static Vector3 _playerPosition = Vector3.zero;

    private const int DISTANCE = 1000;
    #endregion

    #region Public Fields
    public static Vector2Int offset = Vector2Int.zero; //displacement of the coordinate center
    public static bool needDisplacement = false;
    #endregion
    
    
    #region Private Methods
    private void OnEnable()
    {
        offset = SaveManager.GetInstance().playerPosition?.ToVector2Int() ?? Vector2Int.zero;
        StartCoroutine(CheckPosition());
    }

    private IEnumerator CheckPosition()
    {
        while (true)
        {
            _playerPosition = playerTransform.position;
            needDisplacement = Mathf.Abs(_playerPosition.x) >= DISTANCE || Mathf.Abs(_playerPosition.z) >= DISTANCE;
            yield return new WaitForSeconds(1.0f);
        }
    }
    #endregion
    
    #region Public Methods
    public static void AddTransform( Transform transform )
    {
        _transforms.Add(transform);
    }
    
    public static void Displace()
    {
        Vector2Int offsetVector2Int = _playerPosition.ToVector2Int();

        offset += offsetVector2Int;
        
        Vector3 offsetVector3 = new Vector3(offsetVector2Int.x, 0, offsetVector2Int.y);

        foreach (Transform t in _transforms)
        {
            t.position -= offsetVector3;
        }
        
        needDisplacement = false;
    }
    #endregion
}
