/*
    File name: ChangePlayerGravity.cs
    Author: Michael Sweetman
    Summary: Changes the player's gravity when it collides with this game object
    Creation Date: 10/08/2020
    Last Modified: 18/08/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerGravity : MonoBehaviour
{
	public float newGravity = -9.81f;

	private void OnTriggerEnter(Collider other)
	{
		// get the Player Movement component from the collider
		PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
		// if a Player Movement component was found on the collider, the player collided with this game object
		if (playerMovement != null)
		{
			// set the player's gravity to the new gravity
			playerMovement.gravity = newGravity;
		}
	}
}
