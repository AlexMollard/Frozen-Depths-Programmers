/*
    File name: PlayerInteract.cs
    Author:    Luke Lazzaro
    Summary: Enables interaction and opens artifact viewer
    Creation Date: 21/07/2020
    Last Modified: 15/09/2020
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

    // Used for enabling and disabling player movement
    private PlayerMovement pmScript;
    private MouseLook mlScript;

    // Used for enabling tool freezing
    private Tool toolScript;

    private void Awake()
    {
        pmScript = GetComponent<PlayerMovement>();
        mlScript = playerCamera.gameObject.GetComponent<MouseLook>();
        toolScript = GetComponent<Tool>();
    }

    private void Update()
    {
        // Pressing E while standing in front of an interactable enables the artifact viewer for that object
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 camPos = playerCamera.position;

            RaycastHit hit;
            if (Physics.Raycast(camPos, playerCamera.TransformDirection(Vector3.forward), out hit, interactReach, interactableMask))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                Key key = hit.collider.GetComponent<Key>();
                Keyhole keyhole = hit.collider.GetComponent<Keyhole>();

                if (interactable != null)
                {
                    InteractWithArtifact(hit.collider.gameObject);
                }
                else if (key != null)
                {
                    key.Collect();
                }
                else if (keyhole != null)
                {
                    keyhole.Open();
                }
                else if (hit.collider.CompareTag("Tool Component"))
                {
                    toolScript.canFreeze = true;
                    hit.collider.gameObject.SetActive(false);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && pmScript.enabled == false)
        {
            DisableArtifactViewer(true);
        }
    }

    public void InteractWithArtifact(GameObject obj)
    {
        EnableArtifactViewer(obj);

        // Add artifact to save data
        Interactable artifact = obj.GetComponent<Interactable>();

        if (artifact != null && !ArtifactManager.artifactIds.Contains(artifact.id))
            ArtifactManager.artifactIds.Add(artifact.id);

        // Show that the artifact has been found in the UI
        ArtifactDisplay ad = obj.GetComponent<Interactable>().artifactDisplay;
        if (ad != null) ad.Show();
    }

    public void EnableArtifactViewer(GameObject obj)
    {
        artifactViewer.gameObject.SetActive(true);
        viewerDescription.gameObject.SetActive(true);

        artifactViewer.mesh = obj.GetComponent<MeshFilter>().mesh;
        artifactViewer.transform.localScale = obj.transform.localScale;

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

    public void DisableArtifactViewer(bool shouldEnableScripts)
    {
        artifactViewer.gameObject.SetActive(false);
        viewerDescription.gameObject.SetActive(false);

        if (shouldEnableScripts)
        {
            pmScript.enabled = true;
            mlScript.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }
}
