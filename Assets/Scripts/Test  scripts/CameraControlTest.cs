using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlTest : MonoBehaviour
{
    private Camera _camera = null;
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private bool useTestMode = false;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _camera = GetComponent<Camera>();
        if (useTestMode)
            speed = 1000;
    }

    private void Update()
    {
        if (useTestMode)
        {
            Testing();   
        }
        else
        {
            SetSpeed();
            Rotate();
            _camera.transform.position += Move();
        }
    }

    private Vector3 Move()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move += transform.forward;
        
        if (Input.GetKey(KeyCode.S))
            move -= transform.forward;
        
        if (Input.GetKey(KeyCode.D))
            move += transform.right;
        
        if (Input.GetKey(KeyCode.A))
            move -= transform.right;
        
        if (Input.GetKey(KeyCode.R))
            move += transform.up;
        
        if (Input.GetKey(KeyCode.F))
            move -= transform.up;

        return move * Time.deltaTime * speed;
    }

    private void Rotate()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerAngles.x - y , eulerAngles.y + x, eulerAngles.z);
    }

    private void SetSpeed()
    {
        speed += Input.GetAxis("Mouse ScrollWheel") * 30;
        speed = Mathf.Clamp(speed, 0.0f, 1000.0f);
    }

    private void Testing()
    {
        transform.position += Vector3.forward * Time.deltaTime * speed;
    }
}
