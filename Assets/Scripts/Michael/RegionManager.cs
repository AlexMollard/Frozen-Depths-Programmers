/*
    File name: RegionManager.cs
    Author: Michael Sweetman
    Summary: Manages the loading and unloading of different regions of the scene
    Creation Date: 07/10/2020
    Last Modified: 07/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
	[SerializeField] List<GameObject> regions;

	// load the desired region(s) and unload all others
	public void LoadRegion(int regionIndex, bool loadAdjacentRegions)
	{
		// if the region index is in bounds
		if (regionIndex > 0 && regionIndex < regions.Count)
		{
			// unload all regions
			foreach (GameObject region in regions)
			{
				region.SetActive(false);
			}

			// set the desired region to be active
			regions[regionIndex].SetActive(true);
			// if adjacent regions should be loaded
			if (loadAdjacentRegions)
			{
				// if the region is not the first region
				if (regionIndex > 0)
				{
					// load the previous region
					regions[regionIndex - 1].SetActive(true);
				}

				// if the region is not the last region
				if (regionIndex < regions.Count - 1)
				{
					// load the next region
					regions[regionIndex + 1].SetActive(true);
				}
			}
		}
	}
}
