/*
    File name: MetalDetector.cs
    Author:    Luke Lazzaro
    Summary: Detects nearby entrances, and plays a sound based on how close they are.
    Creation Date: 14/09/2020
    Last Modified: 14/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [SerializeField] private Transform entrancesParent;

    [SerializeField] private float maxDistance = 0;
    [SerializeField] private float distanceClose = 0;
    [SerializeField] private float distanceCloser = 0;
    [SerializeField] private float distanceClosest = 0;

    private float currentShortestDistance = float.MaxValue;

    private void Update()
    {
        // Determine which object is closest to the player
        foreach (Transform child in entrancesParent)
        {
            float distance = Vector3.Distance(transform.position, child.position);
            if (child.gameObject.activeSelf && distance <= maxDistance)
            {
                if (distance < currentShortestDistance)
                    currentShortestDistance = distance;
            }
        }

        // Play a sound based on how far the shortest distance is from the player
        // TODO: Add code to play audio when we get sound files
        if (currentShortestDistance < distanceClosest)
            Debug.Log("Closest sound plays!");
        else if (currentShortestDistance < distanceCloser)
            Debug.Log("Closer sound plays!");
        else if (currentShortestDistance < distanceClose)
            Debug.Log("Close sound plays!");

        // Reset distance
        currentShortestDistance = float.MaxValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Disable the entrance so it no longer gets detected
        if (other.tag == "Entrance")
        {
            other.gameObject.SetActive(false);
            Debug.Log("Entrance disabled.");
        }
    }
}
