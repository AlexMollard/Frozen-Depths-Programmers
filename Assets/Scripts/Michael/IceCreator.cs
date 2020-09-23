/*
    File name: IceCreator.cs
    Author: Michael Sweetman
    Summary: determines whether ice creation is valid
    Creation Date: 22/09/2020
    Last Modified: 23/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreator : MonoBehaviour
{
    [SerializeField] GameObject player;
    [HideInInspector] public bool ready;
    [HideInInspector] public EditableTerrain iceTerrain;

    // triggers if a collider enters this object
    private void OnTriggerStay(Collider other)
    {
        // if the other object is tagged with ice
        if (other.tag == "Ice")
        {
            // store the other game object's editable terrain component
            iceTerrain = other.GetComponent<EditableTerrain>();
            // store that the ice creator is ready to store ice
            ready = true;
        }
        // if the other object is the player
        else if (other.gameObject == player)
        {
            // store that the ice creator is not ready to store ice
            ready = false;
        }
    }
}
