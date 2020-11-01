/*
    File name: Checkpoint.cs
    Author:    Luke Lazzaro
    Summary: Saves game data on player collision
    Creation Date: 10/08/2020
    Last Modified: 8/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Should be able to disable checkpoints, and save which checkpoints have been disabled
    public int checkpointNumber = 0;
    public TerrainMan[] terrains;

    private void OnTriggerEnter(Collider other)
    {
        CheckpointManager.checkpointCounter = checkpointNumber + 1;
        CheckpointManager.Instance.UpdateCheckpoints();
        SaveManager.SaveGame(other.gameObject);

        if (CheckpointManager.checkpointCounter > checkpointNumber)
            gameObject.SetActive(false);
    }

    public void ResetMeshes()
    {
        for (int i = 0; i < terrains.Length; i++)
        {
            terrains[i].RestartManager();
        }
    }
}
