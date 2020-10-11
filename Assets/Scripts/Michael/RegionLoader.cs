/*
    File name: RegionLoader.cs
    Author: Michael Sweetman
    Summary: Loads a desired region upon collision with the player
    Creation Date: 07/10/2020
    Last Modified: 07/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionLoader : MonoBehaviour
{
	[SerializeField] RegionManager regionManager;
	[SerializeField] int regionToLoadIndex;
	[SerializeField] bool loadAdjacentRegions = true;

	private void OnTriggerEnter(Collider other)
	{
		// if the collider is the player, load the desired region(s)
		if (other.gameObject.GetComponent<PlayerMovement>() != null)
		{
			regionManager.LoadRegion(regionToLoadIndex, loadAdjacentRegions);
		}
	}
}
