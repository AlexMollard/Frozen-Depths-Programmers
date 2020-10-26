/*
    File name: PlayerMovement.cs
    Author:    Luke Lazzaro
    Summary: Adds first person movement to the player
    Creation Date: 20/07/2020
    Last Modified: 26/10/2020
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
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private float deathTimer = 1;

    [Header("Camera")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private float camHeight = 2;
    [SerializeField] private float camCrouchHeight = 1;

    [Header("Char. Controller")]
    [SerializeField] private float ccHeight = 3;
    [SerializeField] private float ccCrouchHeight = 2;
    //[SerializeField] private float deathVelocity = -20;
    [SerializeField] private GameObject deathUI;

    [Space(10)]
    [Tooltip("Enables flying through scene, and turns off player collider.")]
    [SerializeField] private bool godMode = false;
    [SerializeField] private float flySpeed = 20;

    [HideInInspector] public bool willDie = false;

    private CharacterController controller;
    
    private Vector3 velocity;
    private Vector3 move;
    private bool isGrounded;
    private bool isCrouching = false;
    private Vector3 originalPos;

    // used to store the distance between controller.center and the halfway point on the collider
    private float standCenterHeight = 0f;

    private float currentDeathTimer = 0;
    private bool canMove = true;

    public Vector3 GetMoveVector() { return move; }
    public bool GetGrounded() { return isGrounded; }

    private void Start()
    {
        currentDeathTimer = deathTimer;
        originalPos = transform.position;
        controller = GetComponent<CharacterController>();
        standCenterHeight = (ccHeight - ccCrouchHeight) * 0.5f;

        SyncControllerProperties();
    }

    private void Update()
    {
        if (godMode)
            GodModeMovement();
        else
            NormalMovement();
    }

    private void LateUpdate()
    {
        if (willDie)
        {
            currentDeathTimer -= Time.deltaTime;
            if (currentDeathTimer < 0)
            {
                Respawn();
            }
        }

        isGrounded = false;
    }

    private void NormalMovement()
    {
        if (!canMove) return;

        // Set player to Default layer
        gameObject.layer = 0;

        //Vector3 temp = new Vector3(controller.center.x, controller.height - controller.center.y, controller.center.z);

        Collider[] objectsHit = Physics.OverlapSphere(groundCheck.position, groundCheckRadius);
        foreach (Collider c in objectsHit)
        {
            if (c.transform.root != transform)
            {
                isGrounded = true;
            }
        }

        //if (isGrounded && velocity.y < deathVelocity)
        //{
        //    Die();
        //}

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;

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

    private void GodModeMovement()
    {
        // Set player to NoCollision layer
        gameObject.layer = 10;

        if (Input.GetKey(KeyCode.W))
        {
            controller.Move(transform.forward * flySpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            controller.Move(-transform.right * flySpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            controller.Move(-transform.forward * flySpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            controller.Move(transform.right * flySpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Space))
        {
            controller.Move(transform.up * flySpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            controller.Move(-transform.up * flySpeed * Time.deltaTime);
        }
    }

    private void Crouch()
    {
        if (isCrouching)
        {
            if (Physics.Raycast(transform.position, Vector3.up, standCenterHeight + (ccHeight / 2)))
                return;
        }

        isCrouching = !isCrouching;

        SyncControllerProperties();

        Debug.Log("isGrounded: " + isGrounded);
    }

    // Call this in LateUpdate, otherwise the position will be overwritten by player movement.
    private void GoToLastCheckpoint()
    {
        if (CheckpointManager.currentCheckpoint != null)
        {
            transform.position = CheckpointManager.currentCheckpoint.transform.position;
            transform.rotation = CheckpointManager.currentCheckpoint.transform.rotation;
        }
        else
        {
            transform.position = originalPos;
        }
    }

    public void Die()
    {
        willDie = true;
        deathUI.SetActive(true);
        canMove = false;
        playerCamera.GetComponent<MouseLook>().enabled = false;
    }

    private void Respawn()
    {
        velocity = Vector3.zero;
        GoToLastCheckpoint();
        deathUI.SetActive(false);
        willDie = false;
        currentDeathTimer = deathTimer;
        canMove = true;
        playerCamera.GetComponent<MouseLook>().enabled = true;
    }

    // Overrides any properties currently set on the character controller with the properties set on this script
    private void SyncControllerProperties()
    {
        if (isCrouching)
        {
            controller.height = ccCrouchHeight;
            Vector3 newCenter = new Vector3(controller.center.x, (controller.center.y - (ccHeight - ccCrouchHeight) * 0.5f), controller.center.z);
            controller.center = newCenter;

            float camX = playerCamera.transform.localPosition.x;
            float camZ = playerCamera.transform.localPosition.z;
            Vector3 newCamPos = new Vector3(camX, camCrouchHeight, camZ);
            playerCamera.transform.localPosition = newCamPos;
        }
        else
        {
            controller.height = ccHeight;
            Vector3 newCenter = new Vector3(controller.center.x, (ccHeight - ccCrouchHeight) * 0.5f, controller.center.z);
            controller.center = newCenter;

            float camX = playerCamera.transform.localPosition.x;
            float camZ = playerCamera.transform.localPosition.z;
            Vector3 newCamPos = new Vector3(camX, camHeight, camZ);
            playerCamera.transform.localPosition = newCamPos;
        }

        float gcX = groundCheck.localPosition.x;
        float gcZ = groundCheck.localPosition.z;
        Vector3 newPos = new Vector3(gcX, controller.center.y - controller.height * 0.5f, gcZ);
        groundCheck.localPosition = newPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}
