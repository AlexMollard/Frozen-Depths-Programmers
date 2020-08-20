/*
    File name: PlayerMovement.cs
    Author:    Luke Lazzaro
    Summary: Adds first person movement to the player
    Creation Date: 20/07/2020
    Last Modified: 20/08/2020
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float speed = 12;
    [SerializeField] private float crouchSpeed = 6;
    public float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3;
    [Tooltip("The position of this transform determines where the player touches the ground.")]
    [SerializeField] private Transform groundCheck;
    [Tooltip("The layer for all objects you can walk on.")]
    [SerializeField] private LayerMask groundMask;

    [Header("Camera")]
    [SerializeField] private GameObject playerCamera;
    //[SerializeField] private float camHeight = 2;
    //[SerializeField] private float camCrouchHeight = 1;

    [Header("Char. Controller")]
    [SerializeField] private float ccHeight = 3;
    [SerializeField] private float ccCrouchHeight = 2;

    private CharacterController controller;
    private float groundDistance = 0.4f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Crouch();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (isCrouching)
            controller.Move(move * crouchSpeed * Time.deltaTime);
        else
            controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Crouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            controller.height = ccCrouchHeight;
            controller.center.Set(controller.center.x, 0.5f, controller.center.z);

            //float camX = playerCamera.transform.position.x;
            //float camZ = playerCamera.transform.position.z;
            //playerCamera.transform.position.Set(camX, camCrouchHeight, camZ);
        }
        else
        {
            controller.height = ccHeight;
            controller.center.Set(controller.center.x, 0f, controller.center.z);

            //float camX = playerCamera.transform.position.x;
            //float camZ = playerCamera.transform.position.z;
            //playerCamera.transform.position.Set(camX, camHeight, camZ);
        }
    }
}
