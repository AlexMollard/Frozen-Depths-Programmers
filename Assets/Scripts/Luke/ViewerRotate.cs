using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerRotate : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 50;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 rotVector = new Vector3(Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime, Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime, 0);
            transform.Rotate(rotVector, Space.World);
        }
    }
}
