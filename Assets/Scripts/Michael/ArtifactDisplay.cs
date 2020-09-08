/*
    File name: ArtifactDisplay.cs
    Author: Michael Sweetman
    Summary: Manages the Artifact Display in the in game menu
    Creation Date: 8/09/2020
    Last Modified: 8/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactDisplay : MonoBehaviour
{
    [SerializeField] Sprite hidden;
    [SerializeField] Sprite shown;
    Image display;

    void Start()
    {
        // get the image component
        display = GetComponent<Image>();
        // set the display for the artifact to hidden
        display.sprite = hidden;
    }

    public void Show()
    {
        // set the display for the artifact to shown
        display.sprite = shown;
    }
}
