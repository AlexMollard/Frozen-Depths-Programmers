/*
    File name: SongTransitionTrigger.cs
    Author: Michael Sweetman
    Summary: Manages the transition between songs
    Creation Date: 02/11/2020
    Last Modified: 04/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongTransition : MonoBehaviour
{
    [SerializeField] AudioSource player1;
    [SerializeField] AudioSource player2;
    [SerializeField] List<AudioClip> songs = new List<AudioClip>();
    [SerializeField] MenuManager menuManager;
    [SerializeField] float beatsPerSecond = 0.5f;
    [SerializeField] int beatsPerMeasure = 4;
    [SerializeField] float volumeChangeRate = 2.0f;
    AudioSource currentPlayer;
    AudioSource otherPlayer;

    bool onBeat;
    float timer = 0.0f;
    int beatNumber = 1;
    int songToPlay = -1;
    int currentSongID = 0;
    bool transitioned = true;

    void Start()
    {
        // set the first player to be the current player
        currentPlayer = player1;
        // set the second player to be the other player
        otherPlayer = player2;
        // play the music
        currentPlayer.Play();
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

            // if the bpm is in sync for a new sound effect and there is a song to play
            if (onBeat && songToPlay != -1)
            {
                // if the songToPlay value is valid
                if (songs.Count > songToPlay)
                {
                    // set the other player to play the new song
                    otherPlayer.clip = songs[songToPlay];
                    otherPlayer.Play();
                    // start transitioning from the current song to the new one
                    transitioned = false;
                }
                // reset songToPlay
                songToPlay = -1;
            }
        }

        //if the music is currently transitioning
        if (!transitioned)
        {
            // decrease the volume of the current song
            currentPlayer.volume -= volumeChangeRate * Time.deltaTime;
            // increase the volume of the new song
            otherPlayer.volume += volumeChangeRate * Time.deltaTime;

            // if the new song has reached the desired volume
            if (otherPlayer.volume > menuManager.musicVolume)
            {
                // set the new song to be at exactly the desired volume
                otherPlayer.volume = menuManager.musicVolume;
                // set the volume of the current song to 0 and stop it
                currentPlayer.volume = 0.0f;
                currentPlayer.Stop();
                // store that the music has finished transitioning
                transitioned = true;
                // swap which player is the current and other player
                otherPlayer = currentPlayer;
                currentPlayer = (otherPlayer == player1) ? player2 : player1;
            }
        }
    }

    // stores the argument as the ID for the song to be transitioned to
    public void ChangeSong(int songID)
    {
        // if the ID is not of the currently played song
        if (currentSongID != songID)
        {
            // store the song ID
            songToPlay = songID;
            // store that this is now the ID of the current song
            currentSongID = songID;
            // set the volume of the other player to 0
            otherPlayer.volume = 0.0f;
        }
    }
}
