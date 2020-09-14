/*
    File name: InteractTipManager.cs
    Author: Michael Sweetman
    Summary: Manages the Fuel Bar display on the tool canvas
    Creation Date: 14/09/2020
    Last Modified: 14/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractTipManager : MonoBehaviour
{
    public GameObject display;
    public float range = 10.0f;
    public string interactLayer = "Interactable";
    Camera playerCamera;

    private void Start()
    {
        // get the camera component
        playerCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // cast a ray from the center of the viewport. If it hits an object in the desired layer within range, set the display to be active
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        RaycastHit hit;
        display.SetActive(Physics.Raycast(ray, out hit, range) && hit.collider.gameObject.layer == LayerMask.NameToLayer(interactLayer));
    }
}
