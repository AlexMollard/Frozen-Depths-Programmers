/*
    File name: Footsteps.cs
    Author: Luke Lazzaro
    Summary: Plays specified footstep sounds on specified layers while walking
    Creation Date: 6/11/2020
    Last Modified: 6/11/2020
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FootstepSound
{
    public int layerNumber;
    public AudioClip audioClip;
}

public class Footsteps : MonoBehaviour
{
    // AudioClip slots for footstep sounds, get footstep sounds to test this
    // Should have a sound for each ground layer, might use a list so pat can define these himself
    [SerializeField] private List<FootstepSound> footstepSounds = new List<FootstepSound>();
    private PlayerMovement pm;
    private MouseLook ml;
    private AudioSource footstepSource;
    private bool hasPlayed = false;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        ml = pm.playerCamera.GetComponent<MouseLook>();
        footstepSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        // Check if player is moving and also standing on something
        if ((pm.GetWalking() || pm.GetLanding()) && pm.GetLayerStandingOn() != -1)
        {
            foreach (FootstepSound sound in footstepSounds)
            {
                // Compare the layer the player is walking on with the current sound's layer number, use ground check for this
                if (pm.GetLayerStandingOn() == sound.layerNumber)
                {
                    // If landing, play the sound immediately
                    if (pm.GetLanding())
                    {
                        footstepSource.clip = sound.audioClip;
                        footstepSource.Play();
                    }
                    else
                    {
                        // Play sound only when head bob reaches its lowest point, or when head bob is disabled
                        // This lets pat determine the frequency of the head bob and footsteps at the same time
                        if (ml.amplitude > 0 && ml.sinPosition < -ml.amplitude + ml.amplitude / 5)
                        {
                            if (!hasPlayed)
                            {
                                footstepSource.clip = sound.audioClip;
                                footstepSource.Play();
                                hasPlayed = true;
                            }
                        }
                        else
                        {
                            hasPlayed = false;
                        }
                    }
                }
            }
        }
    }
}
