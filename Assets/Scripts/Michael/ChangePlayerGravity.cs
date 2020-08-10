using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerGravity : MonoBehaviour
{
	public float newGravity = -9.81f;

	private void OnTriggerEnter(Collider other)
	{
		PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
		if (playerMovement != null)
		{
			playerMovement.gravity = newGravity;
		}
	}
}
