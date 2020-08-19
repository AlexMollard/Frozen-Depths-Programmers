/*
    File name: ViewerRotate.cs
    Author:    Luke Lazzaro
    Summary: Allows rotation of the artifact viewer with the mouse
    Creation Date: 22/07/2020
    Last Modified: 22/07/2020
*/

using System;
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
            // Rotates the object relative to the camera's position
            Vector3 rotVector = new Vector3(Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime, Input.GetAxis("Mouse X") * -mouseSensitivity * Time.deltaTime, 0);
            rotVector = transform.parent.TransformVector(rotVector);
            transform.Rotate(rotVector, Space.World);
        }
    }
}
