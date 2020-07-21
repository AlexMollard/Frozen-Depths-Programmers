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
                // Copy mesh from interactable, display it in front of player, disable player movement and enable mouse movement
                // until interaction cancelled.

                Mesh newMesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;
                EnableArtifactViewer(newMesh);
            }
        }
    }

    private void EnableArtifactViewer(Mesh mesh)
    {
        artifactViewer.gameObject.SetActive(true);
        artifactViewer.mesh = mesh;
        pmScript.enabled = false;
        mlScript.enabled = false;
    }
}
