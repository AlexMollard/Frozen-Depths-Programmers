using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Should be able to disable checkpoints, and save which checkpoints have been disabled
    public int checkpointNumber = 0;

    private void OnTriggerEnter(Collider other)
    {
        CheckpointManager.checkpointCounter = checkpointNumber + 1;
        CheckpointManager.Instance.UpdateCheckpoints();
        SaveManager.SaveGame(other.gameObject);

        if (CheckpointManager.checkpointCounter > checkpointNumber)
            gameObject.SetActive(false);
    }
}
