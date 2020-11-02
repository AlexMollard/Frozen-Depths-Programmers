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
			songTransition.ChangeSong(songID);
		}
	}
}
