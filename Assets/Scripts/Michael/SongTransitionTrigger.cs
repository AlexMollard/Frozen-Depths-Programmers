/*
    File name: SongTransitionTrigger.cs
    Author: Michael Sweetman
    Summary: Changes music to a desired song after colliding this with the player
    Creation Date: 02/11/2020
    Last Modified: 04/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongTransitionTrigger : MonoBehaviour
{
    [SerializeField] int songID;
    [SerializeField] SongTransition songTransition;

	private void OnTriggerEnter(Collider other)
	{
		// get the Player Movement component from the collider
		PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
		// if a Player Movement component was found on the collider, the player collided with this game object
		if (playerMovement != null)
		{
			// tell the songTransition script to change the song to the song represented by the songID
			songTransition.ChangeSong(songID);
		}
	}
}
