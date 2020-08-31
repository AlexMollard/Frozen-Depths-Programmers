/*
    File name: Keyhole.cs
    Author:    Luke Lazzaro
    Summary: Does something if the player has a required key
    Creation Date: 31/08/2020
    Last Modified: 31/08/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum OpenBehaviour
{
    RisingDoor
}

public class Keyhole : MonoBehaviour
{
    [SerializeField] private string id = "";
    [SerializeField] private GameObject objectToOpen;
    [SerializeField] private OpenBehaviour openBehaviour = OpenBehaviour.RisingDoor;

    private void Start()
    {
        if (string.IsNullOrEmpty(id))
            Debug.LogError("One of your keyholes doesn't have an ID!");
    }

    public void Open()
    {
        if (!KeyManager.keys.Contains(id))
        {
            Debug.Log("No key matches this keyhole.");
            return;
        }

        switch (openBehaviour)
        {
            case OpenBehaviour.RisingDoor:
                Debug.Log("Opening door...");
                // TODO: Use Vector3.MoveTowards for a more realistic look
                float posX = objectToOpen.transform.position.x;
                float posY = objectToOpen.transform.position.y;
                float posZ = objectToOpen.transform.position.z;
                objectToOpen.transform.position = new Vector3(posX, posY + 5, posZ);
                break;
            default:
                break;
        }
    }
}
