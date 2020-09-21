/*
    File name: Tool.cs
    Author: Michael Sweetman
    Summary: Determines a point on the ice mesh the player wants burnt/frozen. Manages a fuel to limit the use of ice creation.
    Creation Date: 21/07/2020
    Last Modified: 15/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [Header("Tool Use")]
    public bool canFreeze = false;
    public float minimumFreezeDistance = 2.0f;
    public float maxRange = 10.0f;
    public float beamRadius = 0.5f;
    public float effectRadius = 10.0f;
    public float tickRate = 0.0f;
    float tickTimer = 0.0f;

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
    public GameObject laser;
    public Transform laserStartPoint;
    public Material burnLaserMaterial;
    public Material freezeLaserMaterial;
    MeshRenderer laserRenderer;
    float laserLengthScalar;

    private void Start()
    {
        // get the mesh renderer for the laser
        laserRenderer = laser.GetComponent<MeshRenderer>();

        // get the relative length the display beam needs to be relative to the actual spherecast distance. Half this value as as scale 1 cylinder is 2 units long
        laserLengthScalar = (new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, playerCamera.transform.position.z + maxRange) - laserStartPoint.position).magnitude / maxRange * 0.5f;
    }

    void Update()
    {
        // set the laser to be inactive
        laser.SetActive(false);
        // increase the timer by the amount of time passed since last frame
        tickTimer += Time.deltaTime;

        // if the mouse is left clicked, or right clicked with fuel
        if ((Input.GetMouseButton(0)|| (Input.GetMouseButton(1) && toolFuel > 0.0f)))
        {
            // if the tool can freeze, or if the mouse was left clicked
            if (canFreeze || Input.GetMouseButton(0))
            {
                // activate the laser
                laser.SetActive(true);
                // set the material of the laser based on the the tool's fire mode
                laserRenderer.material = (Input.GetMouseButton(0)) ? burnLaserMaterial : freezeLaserMaterial;
            }

            // if enough time has passed since the last terrain edit
            if (tickTimer > tickRate)
            {
                // reset the tick timer
                tickTimer = 0.0f;

                // cast a spherecast forward from the center of the player camera viewport
                Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
                RaycastHit hit;
                // if the spherecast hit a game object
                if (Physics.SphereCast(ray, beamRadius, out hit, maxRange))
                {
                    // position and scale the laser so it fires from the laser start point with a length based on the hit distance and a radius based on the beam radius
                    laser.transform.position = laserStartPoint.position + laserStartPoint.forward * hit.distance * laserLengthScalar;
                    laser.transform.localScale = new Vector3(laser.transform.localScale.x, hit.distance * laserLengthScalar, laser.transform.localScale.z);

                    // if the hit gameobject has the tag "Ice"
                    if (hit.transform.tag == "Ice")
                    {
                        // if the mouse was left clicked
                        if (Input.GetMouseButton(0))
                        {
                            // burn the ice at the point of the collision. If this succeeds and the tool is able to freeze ice
                            if (hit.transform.GetComponent<EditableTerrain>().EditTerrain(false, hit.point, effectRadius, toolStrength) && canFreeze)
                            {
                                // increase the fuel by the fuel gain rate per second
                                toolFuel += Time.deltaTime * FuelGainRate;
                                // if there is a capacity and the tool fuel is above that capacity
                                if (capacity > 0.0f && toolFuel > capacity)
                                {
                                    // set the fuel be equal to the capacity
                                    toolFuel = capacity;
                                }
                                hit.transform.GetComponent<EditableTerrain>().manager.dirtyChunks.Clear();
                            }
                        }
                        // if the tool is able to freeze ice, the mouse was right clicked and the collision point was beyond the minimum creation distance
                        else if (canFreeze && hit.distance >= minimumFreezeDistance)
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
                                hit.transform.GetComponent<EditableTerrain>().manager.dirtyChunks.Clear();
                            }
                        }
                    }
                }
                // if the spherecast did not hit a game object 
                else
                {
                    // position and scale the laser so it fires from the laser start point with a length based on the max range and a radius based on the beam radius
                    laser.transform.position = laserStartPoint.position + laserStartPoint.forward * maxRange * laserLengthScalar;
                    laser.transform.localScale = new Vector3(laser.transform.localScale.x, maxRange * laserLengthScalar, laser.transform.localScale.z);
                }
            }
        }
    }
}
