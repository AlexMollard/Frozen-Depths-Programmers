/*
    File name: MovingImage.cs
    Author: Michael Sweetman
    Summary: A UI image that moves and scales to its artifact display before replacing the artifact display's image and deleting itself
    Creation Date: 04/11/2020
    Last Modified: 09/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingImage : MonoBehaviour
{
    [HideInInspector] public ArtifactDisplay artifactDisplay;
    Vector3 startPosition;
    Vector2 startDimensions;
    Vector3 targetPosition;
    Vector2 targetDimensions;
    public float duration = 1.0f;
    RectTransform rectTransform;
    float timer = 0.0f;

    private void Start()
    {
        // determine the start and end position for the lerp
        startPosition = transform.position;
        targetPosition = artifactDisplay.transform.position;

        // determine the start and end dimensions for the lerp
        rectTransform = GetComponent<RectTransform>();
        startDimensions = rectTransform.sizeDelta;
        targetDimensions = artifactDisplay.GetComponent<RectTransform>().sizeDelta;

        // set the image to be the artifact display's shown image
        GetComponent<Image>().sprite = artifactDisplay.shown;
    }

    void Update()
    {
        // increase the timer such that the timer reaches 1 once duration seconds have passed
        timer += Time.deltaTime / duration;
        // lerp the position from the start position to the target position using the timer
        transform.position = Vector3.Lerp(startPosition, targetPosition, timer);
        // lerp the dimensions from the start dimensions to the target dimensions using the timer
        rectTransform.sizeDelta = Vector2.Lerp(startDimensions, targetDimensions, timer);

        // if the timer has reached 1
        if (timer >= 1.0f)
        {
            // get the image component of the artifact display
            Image artifactDisplayImage = artifactDisplay.GetComponent<Image>();
            // set the artifact display's image to be its shown image
            artifactDisplayImage.sprite = artifactDisplay.shown;
            // set the artifact display's alpha to be its foundAlpha
            artifactDisplayImage.color = new Color(artifactDisplayImage.color.r, artifactDisplayImage.color.g, artifactDisplayImage.color.b, artifactDisplay.foundAlpha);
            // destroy this game object
            Destroy(gameObject);
        }
    }
}
