/*
    File name: MetalDetector.cs
    Author:    Luke Lazzaro
    Summary: Detects nearby entrances, and plays a sound based on how close they are.
    Creation Date: 14/09/2020
    Last Modified: 21/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [SerializeField] private Transform entrancesParent;

    [SerializeField] private float maxDistance = 0;
    [Space(10)]
    [SerializeField] private float distanceClose = 0;
    [SerializeField] private AudioClip closeSound;
    [Space(10)]
    [SerializeField] private float distanceCloser = 0;
    [SerializeField] private AudioClip closerSound;
    [Space(10)]
    [SerializeField] private float distanceClosest = 0;
    [SerializeField] private AudioClip closestSound;

    private float currentShortestDistance = float.MaxValue;
    private float previousShortestDistance = 0;
    private AudioSource audioPlayer;

    public float GetDistance()
    {
        return previousShortestDistance;
    }

    private void Awake()
    {
        audioPlayer = gameObject.AddComponent<AudioSource>();
    }

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
        {
            // Play sound if not already playing
            if (!audioPlayer.isPlaying)
            {
                audioPlayer.clip = closestSound;
                audioPlayer.Play();
            }
        }
        else if (currentShortestDistance < distanceCloser)
        {
            if (!audioPlayer.isPlaying)
            {
                audioPlayer.clip = closerSound;
                audioPlayer.Play();
            }
        }
        else if (currentShortestDistance < distanceClose)
        {
            if (!audioPlayer.isPlaying)
            {
                audioPlayer.clip = closeSound;
                audioPlayer.Play();
            }
        }

        // Reset distance
        previousShortestDistance = currentShortestDistance;
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
