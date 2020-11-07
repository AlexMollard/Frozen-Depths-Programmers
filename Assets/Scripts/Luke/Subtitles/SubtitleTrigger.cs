/*
    File name: SubtitleTrigger.cs
    Author:    Luke Lazzaro
    Summary: Trigger for calling methods on Subtitles.cs
    Creation Date: 27/07/2020
    Last Modified: 7/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{
    [SerializeField] private Subtitles subtitlesObject;
    public int cpToDestroyAt = 100;
    [TextArea] [SerializeField] private string newText = "";
    [SerializeField] private AudioClip voiceActing;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            subtitlesObject.UpdateSubtitles(newText);
            subtitlesObject.PlayVoiceActing(voiceActing);
            gameObject.SetActive(false);
        }
    }
}
