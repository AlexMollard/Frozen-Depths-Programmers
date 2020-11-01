/*
    File name: CheckpointManager.cs
    Author:    Luke Lazzaro
    Summary: Handles the current checkpoint counter and updates checkpoints
    Creation Date: 10/08/2020
    Last Modified: 8/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    // The current checkpoint counter. The current value represents the last checkpoint you touched, not how many you've passed.
    // All previous checkpoints will be disabled when updated.
    public static int checkpointCounter = 0;
    public static GameObject currentCheckpoint;

    public ManagerOfTheTerrainManagers terrainMan;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void UpdateCheckpoints()
    {
        foreach (Transform child in transform)
        {
            // Update checkpoints based on checkpoint counter
            Checkpoint current = child.gameObject.GetComponent<Checkpoint>();

            // Set Managers
            current.terrains = terrainMan.GetManagers(current.checkpointNumber);

            // Set the current checkpoint based on checkpointCounter
            if (current.checkpointNumber == checkpointCounter - 1)
                currentCheckpoint = current.gameObject;

            if (current.checkpointNumber < checkpointCounter)
                current.gameObject.SetActive(false);
            else
                current.gameObject.SetActive(true);
        }
    }
}
