/*
    File name: FadeScreen.cs
    Author: Michael Sweetman
    Summary: Tells a screen to start fading when a player collides with it
    Creation Date: 13/10/2020
    Last Modified: 13/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
	[SerializeField] Screen screen;
	[SerializeField] float duration = 0.2f;
	[SerializeField] bool makeOpaque = true;
	[SerializeField] bool destroyAfterTrigger = true;

	private void OnTriggerEnter(Collider other)
	{
		// get the Player Movement component from the collider
		PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
		// if a Player Movement component was found on the collider, the player collided with this game object
		if (playerMovement != null)
		{
			// tell the screen to start fading
			screen.Fade(duration, makeOpaque);

			// if this object should be destroyed
			if (destroyAfterTrigger)
			{
				// destroy this game object
				Destroy(this.gameObject);
			}
		}
	}
}
