using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    public static int checkpointCounter = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // This will check the current checkpoint counter and enable/disable checkpoints accordingly. Currently used when loading the game.
    public void UpdateCheckpoints()
    {
        foreach (Transform child in transform)
        {
            // Will this still work if the object is currently disabled?
            Checkpoint current = child.gameObject.GetComponent<Checkpoint>();

            if (current.checkpointNumber < checkpointCounter)
                current.gameObject.SetActive(false);
            else
                current.gameObject.SetActive(true);
        }
    }
}
