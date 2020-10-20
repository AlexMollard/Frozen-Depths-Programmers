/*
    File name: IceCreator.cs
    Author: Michael Sweetman
    Summary: determines whether ice creation is valid
    Creation Date: 22/09/2020
    Last Modified: 20/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreator : MonoBehaviour
{
    [SerializeField] GameObject player;
    [HideInInspector] public bool ready;
    [HideInInspector] public EditableTerrain iceTerrain;
    [HideInInspector] public Vector3 collisionPoint;

    // triggers if a collider enters this object
    private void OnTriggerStay(Collider other)
    {
        // if the other object is tagged with ice
        if (other.tag == "Ice")
        {
            // store the other game object's editable terrain component
            iceTerrain = other.GetComponent<EditableTerrain>();
            // store the point on the collider to the centre of the ice creator as the collision point
            collisionPoint = other.ClosestPoint(transform.position);
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
