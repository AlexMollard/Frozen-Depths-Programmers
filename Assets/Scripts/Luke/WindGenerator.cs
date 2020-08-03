/*
    File name: WindGenerator.cs
    Author:    Luke Lazzaro
    Summary: Moves specified wind objects by a force in a direction
    Creation Date: 29/07/2020
    Last Modified: 3/08/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> windObjects;
    [SerializeField] private float windSpeed = 2;

    // Update is called once per frame
    void Update()
    {
        // The direction of the wind will be determined by the Z axis (transform.forward)
        // The generator will call a movement method on the objects every frame in a for loop
        foreach (GameObject obj in windObjects)
        {
            obj.GetComponent<WindObject>().Move(transform.forward, windSpeed);
        }
    }
}
