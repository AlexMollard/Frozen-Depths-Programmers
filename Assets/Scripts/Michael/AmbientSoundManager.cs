/*
    File name: RandomAudio.cs
    Author: Michael Sweetman
    Summary: Ensures that enqueued audio clips play on the first beat of a measure
    Creation Date: 05/10/2020
    Last Modified: 12/10/2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [SerializeField] float beatsPerSecond = 0.5f;
    [SerializeField] int beatsPerMeasure = 4;
    [SerializeField] AudioSource musicPlayer;

    Queue<AudioSource> soundQueue = new Queue<AudioSource>();
    bool onBeat;
    float timer = 0.0f;
    int beatNumber = 1;

    // adds a sound to the sound queue
    public void AddToQueue(AudioSource player)
    {
        // add the audio player to the sound queue
        soundQueue.Enqueue(player);
    }

    private void Start()
    {
        // play the music
        musicPlayer.Play();
    }

    void Update()
    {
        // increase the timer by the time that passed since last frame
        timer += Time.deltaTime;
        // if a beat should have occured
        if (timer >= beatsPerSecond)
        {
            // reset the timer, saving the excess time for next loop
            timer -= beatsPerSecond;
            
            // store the new beat number
            beatNumber++;

            // if this is the first beat of the measure, then new sound effects should play
            onBeat = (beatNumber % beatsPerMeasure == 0);
            
            // if the bpm is in sync for a new sound effect to play and the player is not currently playing a sound
            if (onBeat && soundQueue.Count > 0 && !soundQueue.Peek().isPlaying)
            {
                // remove the sound from the queue and play it
                soundQueue.Dequeue().Play();
            }
        }
    }
}
