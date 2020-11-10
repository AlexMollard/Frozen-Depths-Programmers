/*
    File name: MouseLook.cs
    Author:    Luke Lazzaro
    Summary: Rotates a first person camera with the mouse
    Creation Date: 20/07/2020
    Last Modified: 10/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100;
    [SerializeField] private Transform playerBody;

    [Header("Head Bob Settings")]
    public float amplitude = 1;
    public float frequency = 0.05f;
    [HideInInspector] public float sinPosition = 0;

    private float xRotation = 0;
    private float sinCounter = 0;

    void Start()
    {
        // Lock cursor by default
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void FixedUpdate()
    {
    	PlayerMovement pm = playerBody.GetComponent<PlayerMovement>();
        Vector3 playerMove = pm.GetMoveVector();

        if (Vector3.Magnitude(playerMove) > 0.1f && pm.GetGrounded())
        {
            sinPosition = Mathf.Sin(frequency * sinCounter) * amplitude;
            float newY = transform.position.y + sinPosition;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            sinCounter++;
        }
    }
}
