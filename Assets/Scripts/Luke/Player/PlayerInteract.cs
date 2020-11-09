/*
    File name: PlayerInteract.cs
    Author:    Luke Lazzaro
    Summary: Enables interaction and opens artifact viewer
    Creation Date: 21/07/2020
    Last Modified: 9/11/2020
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
    [SerializeField] private GameObject gunObject;
    [SerializeField] private GameObject freezerAttachment;
    [SerializeField] private AudioClip antidotePickupSound;
    [SerializeField] private MenuManager menuManager;

    [Header("Tutorials")]
    [SerializeField] private GameObject meltTutorial;
    [SerializeField] private GameObject createTutorial;

    // Used for enabling and disabling player movement
    private PlayerMovement pmScript;
    private MouseLook mlScript;

    // Used for enabling tool freezing
    private Tool toolScript;

    private AudioSource antidotePickupSource;

    private void Awake()
    {
        pmScript = GetComponent<PlayerMovement>();
        mlScript = playerCamera.gameObject.GetComponent<MouseLook>();
        toolScript = GetComponent<Tool>();
        antidotePickupSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        // Pressing E while standing in front of an interactable enables the artifact viewer for that object
        if (Input.GetKeyDown(KeyCode.E))
        {
            // remove any active tutorials
            meltTutorial.SetActive(false);
            createTutorial.SetActive(false);

            Vector3 camPos = playerCamera.position;

            RaycastHit hit;

            if (!pmScript.enabled)
            {
                DisableArtifactViewer(true);
            }
            else if (Physics.Raycast(camPos, playerCamera.TransformDirection(Vector3.forward), out hit, interactReach, interactableMask))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                Key key = hit.collider.GetComponent<Key>();
                Keyhole keyhole = hit.collider.GetComponent<Keyhole>();
                Antidote antidote = hit.collider.GetComponent<Antidote>();

                if (interactable != null)
                {
                    InteractWithArtifact(hit.collider.gameObject);
                    hit.collider.gameObject.SetActive(false);
                }
                else if (key != null)
                {
                    key.Collect();
                }
                else if (keyhole != null)
                {
                    keyhole.PlaceKeyOnKeyhole();
                }
                else if (antidote != null)
                {
                    antidotePickupSource.clip = antidotePickupSound;
                    antidotePickupSource.Play();

                    antidote.Collect();
                }
                else if (hit.collider.CompareTag("gun"))
                {
                    gunObject.SetActive(true);
                    toolScript.enabled = true;
                    meltTutorial.SetActive(true);
                    menuManager.playerHasTool = true;
                    hit.collider.gameObject.SetActive(false);
                }
                else if (hit.collider.CompareTag("Tool Component"))
                {
                    toolScript.canFreeze = true;
                    hit.collider.gameObject.SetActive(false);
                    createTutorial.SetActive(true);
                    freezerAttachment.SetActive(false);
                }
            }
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
        artifactViewer.gameObject.transform.LookAt(playerCamera);
        viewerDescription.gameObject.SetActive(true);

        artifactViewer.mesh = obj.GetComponent<MeshFilter>().mesh;
        artifactViewer.gameObject.GetComponent<MeshRenderer>().material = obj.GetComponent<MeshRenderer>().material;
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
        toolScript.enabled = false;

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

            if (menuManager.playerHasTool)
            {
                toolScript.enabled = true;
            }

        }

        Cursor.lockState = CursorLockMode.Locked;
    }
}
