/*
    File name: Tool.cs
    Author: Michael Sweetman
    Summary: Determines a point on the ice mesh the player wants burnt/frozen. Manages a fuel to limit the use of ice creation.
    Creation Date: 21/07/2020
    Last Modified: 8/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [Header("Tool Use")]
    public float minimumFreezeDistance = 2.0f;
    public float maxRange = 10.0f;
    public float beamRadius = 0.5f;
    public float effectRadius = 10.0f;
    public float stopWatchTime = 0.15f;
    float timer = 0.0f;

    [Header("Fuel Economy")]
    public float FuelGainRate = 100.0f;
    public float FuelLossRate = 100.0f;
    public float capacity = 1000.0f;
    public float toolStrength = 0.1f;
    [HideInInspector]
    public float toolFuel = 0.0f;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Laser")]
    public LineRenderer laser;
    public Transform laserStartPoint;
    public Material burnLaserMaterial;
    public Material freezeLaserMaterial;

    void Update()
    {
        laser.enabled = false;
        laser.SetPosition(0, laserStartPoint.position);
        timer += Time.deltaTime;

        // if the mouse is left clicked or if the mouse is right clicked and the tool has fuel
        if ((Input.GetMouseButton(0)|| Input.GetMouseButton(1)) && timer > stopWatchTime)
        {
            laser.enabled = true;
            laser.material = (Input.GetMouseButton(0)) ? burnLaserMaterial : freezeLaserMaterial;

            timer = 0.0f;
            // cast a ray forward from the center of the player camera viewport
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,1.0f));
            RaycastHit hit;

            // if the ray hit a game object
            if (Physics.SphereCast(ray, beamRadius, out hit, maxRange))
            {
                laser.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * hit.distance * 0.75f);

                // if the hit gameobject has the tag "Ice"
                if (hit.transform.tag == "Ice")
                {
                    // if the mouse was left clicked
                    if (Input.GetMouseButton(0))
                    {
                        // burn the ice at the point of the collision. If this succeeds
                        if (hit.transform.GetComponent<EditableTerrain>().EditTerrain(false, hit.point, effectRadius, toolStrength))
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
                    // if the mouse was right clicked and the collision point was beyond the minimum creation distance
                    else if (hit.distance >= minimumFreezeDistance)
                    {
                        // freeze the ice at the point of the collision. If this succeeds
                        if (hit.transform.GetComponent<EditableTerrain>().EditTerrain(true, hit.point, effectRadius, toolStrength))
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
            }
            else
            {
                laser.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * maxRange * 0.75f);
            }
        }
    }
}
