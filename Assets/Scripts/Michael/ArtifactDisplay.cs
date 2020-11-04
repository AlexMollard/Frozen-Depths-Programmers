/*
    File name: ArtifactDisplay.cs
    Author: Michael Sweetman
    Summary: Manages the Artifact Display in the in game menu
    Creation Date: 08/09/2020
    Last Modified: 04/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactDisplay : MonoBehaviour
{
    [SerializeField] GameObject movingImage;
    public Sprite shown;
    bool called = false;

    public void Show()
    {
        // this function hasn't been called before
        if (!called)
        {
            // store that this is the moving image's artifact display
            movingImage.GetComponent<MovingImage>().artifactDisplay = this;
            // create a moving image at the center of the screen, with 0 rotation and the parent of this game object
            Instantiate(movingImage, transform.parent.transform.parent.position, Quaternion.identity, transform.parent);
            // store that this function has been called
            called = true;
        }
    }
}
