/*
    File name: Crosshair.cs
    Author: Michael Sweetman
    Summary: Sets the image used to represent the crosshair
    Creation Date: 27/10/2020
    Last Modified: 27/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Sprite withinRangeImage;
    [SerializeField] Sprite outOfRangeImage;
    Image crosshairRenderer;

    void Start()
    {
        // get the mesh renderer component
        crosshairRenderer = GetComponent<Image>();
    }

    // sets the image used to represent the crosshair
    public void SetImage(bool withinRange)
    {
        // if within range, use the within range image to represent the crosshair, otherwise use the out of range image
        crosshairRenderer.sprite = (withinRange) ? withinRangeImage : outOfRangeImage;
    }
}
