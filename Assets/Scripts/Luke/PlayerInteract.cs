/*
    File name: PlayerInteract.cs
    Author:    Luke Lazzaro
    Summary: Enables interaction and opens artifact viewer
    Creation Date: 21/07/2020
    Last Modified: 22/07/2020
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float interactReach = 10;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private MeshFilter artifactViewer;
    [SerializeField] private Text viewerDescription;

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
                EnableArtifactViewer(hit.collider.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && pmScript.enabled == false)
        {
            DisableArtifactViewer();
        }
    }

    public void EnableArtifactViewer(GameObject obj)
    {
        artifactViewer.gameObject.SetActive(true);
        viewerDescription.gameObject.SetActive(true);

        artifactViewer.mesh = obj.GetComponent<MeshFilter>().mesh;

        try
        {
            viewerDescription.text = obj.GetComponent<Interactable>().description;
        }
        catch
        {
            Debug.LogError("Interactable objects must have the Interactable.cs script attached to work properly.");
        }

        pmScript.enabled = false;
        mlScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableArtifactViewer()
    {
        artifactViewer.gameObject.SetActive(false);
        viewerDescription.gameObject.SetActive(false);

        pmScript.enabled = true;
        mlScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
    }
}
