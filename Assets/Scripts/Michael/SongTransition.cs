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
        currentPlayer = player1;
        otherPlayer = player2;
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
                if (songs.Count > songToPlay)
                {
                    otherPlayer.clip = songs[songToPlay];
                    otherPlayer.Play();
                    transitioned = false;
                }
                songToPlay = -1;
            }
        }

        if (!transitioned)
        {
            currentPlayer.volume -= volumeChangeRate * Time.deltaTime;
            otherPlayer.volume += volumeChangeRate * Time.deltaTime;

            if (otherPlayer.volume > menuManager.musicVolume)
            {
                otherPlayer.volume = menuManager.musicVolume;
                currentPlayer.volume = 0.0f;
                transitioned = true;
                otherPlayer = currentPlayer;
                currentPlayer = (otherPlayer == player1) ? player2 : player1;
            }
        }
    }

    public void ChangeSong(int songID)
    {
        if (currentSongID != songID)
        {
            songToPlay = songID;
            currentSongID = songID;
        }
    }
}
