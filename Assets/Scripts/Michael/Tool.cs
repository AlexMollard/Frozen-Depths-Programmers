/*
    File name: Tool.cs
    Author: Michael Sweetman
    Summary: Determines a point on the ice mesh the player wants burnt/frozen. Manages a fuel to limit the use of ice creation.
    Creation Date: 21/07/2020
    Last Modified: 18/08/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour
{
    [Header("Tool Use")]
    public float range = 10.0f;
    public float radius;

    [Header("Fuel Economy")]
    [HideInInspector]
    public float toolFuel = 0.0f;
    public float FuelGainRate = 100.0f;
    public float FuelLossRate = 100.0f;
    public float capacity = 1000.0f;

    [Header("FuelDisplay")]
    public Text fuelDisplay;

    [Header("Camera")]
    public Camera playerCamera;

    void Update()
    {
        // if the mouse is left clicked or if the mouse is right clicked and the tool has fuel
        if (Input.GetMouseButton(0)|| Input.GetMouseButton(1) && toolFuel > 0.0f)
        {
            // cast a ray forward from the center of the player camera viewport
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,1.0f));
            RaycastHit hit;
            // if the ray hit a game object with the tag "Ice"
            if (Physics.Raycast(ray, out hit, range) && hit.transform.tag == "Ice")
            {
                // if the mouse was left clicked
                if (Input.GetMouseButton(0))
                {
                    // burn the ice at the point of the collision. If this succeeds
                    if (hit.transform.GetComponent<EditableTerrain>().Burn(hit.point, radius))
                    {
                        // increase the fuel by the fuel gain rate per second
                        toolFuel += Time.deltaTime * FuelGainRate;
                        // if there is a capacity and the tool fuel is above that capacity
                        if (capacity > 0.0f && toolFuel > capacity)
                        {
                            // set the fuel be equal to the capacity
                            toolFuel = capacity;
                        }
                    }
                }
                // if the mouse was right clicked
                else
                {
                    // freeze the ice at the point of the collision. If this succeeds
                    if (hit.transform.GetComponent<EditableTerrain>().Freeze(hit.point, radius))
                    {
                        // decrease the the fuel by the fuel loss rate per second
                        toolFuel -= Time.deltaTime * FuelLossRate;
                        // if there is less than 0 fuel
                        if (toolFuel < 0.0f)
                        {
                            // set the fuel to be 0
                            toolFuel = 0.0f;
                        }
                    }
                }
            }
            // update the fuel display text
            fuelDisplay.text = toolFuel.ToString("F2") + "mL";
        }
    }
}
