﻿/*
    File name: PlayerInteract.cs
    Author:    Luke Lazzaro
    Summary: Enables interaction and opens artifact viewer
    Creation Date: 21/07/2020
    Last Modified: 22/07/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float interactReach = 10;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private MeshFilter artifactViewer;

    private PlayerMovement pmScript;
    private MouseLook mlScript;

    private void Awake()
    {
        pmScript = GetComponent<PlayerMovement>();
        mlScript = playerCamera.gameObject.GetComponent<MouseLook>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 camPos = playerCamera.position;

            RaycastHit hit;
            if (Physics.Raycast(camPos, playerCamera.TransformDirection(Vector3.forward), out hit, interactReach, interactableMask))
            {
                Mesh newMesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;
                EnableArtifactViewer(newMesh);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DisableArtifactViewer();
        }
    }

    public void EnableArtifactViewer(Mesh mesh)
    {
        artifactViewer.gameObject.SetActive(true);
        artifactViewer.mesh = mesh;
        pmScript.enabled = false;
        mlScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableArtifactViewer()
    {
        artifactViewer.gameObject.SetActive(false);
        pmScript.enabled = true;
        mlScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
    }
}
