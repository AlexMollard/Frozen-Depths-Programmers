using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void UpdateSubtitles()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<SubtitleTrigger>().cpToDestroyAt <= CheckpointManager.checkpointCounter)
                child.gameObject.SetActive(false);
            else
                child.gameObject.SetActive(true);
        }
    }
}
