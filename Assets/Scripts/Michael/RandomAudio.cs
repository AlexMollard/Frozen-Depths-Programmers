/*
    File name: RandomAudio.cs
    Author: Michael Sweetman
    Summary: intermittently plays a random sound effect from a set list
    Creation Date: 05/10/2020
    Last Modified: 12/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudio : MonoBehaviour
{
    [SerializeField] List<AudioClip> sounds;
    [SerializeField] AudioSource audioPlayer;
    public bool loop = false;
    [SerializeField] float minTime = 3.0f;
    [SerializeField] float maxTime = 10.0f;
    bool playedSound = false;
    float timer;

    private void Start()
    {
        // determine a random amount of time before the sound effect plays (to be used if the sound effect is set to loop)
        timer = Random.Range(minTime, maxTime);
    }

    private void Update()
    {
        // if the audio should play repetitively
        if (loop)
        {
            // reduce the timer by the time that passed since last frame
            timer -= Time.deltaTime;
            // if the timer has ran out
            if (timer <= 0)
            {
                // if the audio player is not currently playing a sound
                if (!audioPlayer.isPlaying)
                {
                    // if a sound was set to play and just finished
                    if (playedSound)
                    {
                        // determine a random amount of time before the next sound effect plays
                        timer = Random.Range(minTime, maxTime);
                        // reset the playedSound flag
                        playedSound = false;
                    }
                    // if a sound is yet to play
                    else
                    {
                        // play a random sound
                        playRandomSound();
                        // store that a random sound has been set to play
                        playedSound = true;
                    }

                }
            }
        }
    }

    // plays a random sound effect from the sounds list
    public void playRandomSound()
    {
        // if the audio player is not currently playing a sound
        if (!audioPlayer.isPlaying)
        {
            // set the audio player's clip to be a random sound effect in the sounds list
            audioPlayer.clip = sounds[Random.Range(0, sounds.Count - 1)];
            // play the audio player's clip
            audioPlayer.Play();
        }
    }
}
