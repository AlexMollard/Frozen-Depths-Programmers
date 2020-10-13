/*
    File name: Tool.cs
    Author: Michael Sweetman
    Summary: Determines a point on the ice mesh the player wants burnt/frozen. Manages a fuel to limit the use of ice creation.
    Creation Date: 21/07/2020
    Last Modified: 13/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Tool : MonoBehaviour
{
    [Header("Tool Use")]
    [SerializeField] float minimumFreezeDistance = 2.0f;
    [SerializeField] float maxRange = 10.0f;
    [SerializeField] float beamRadius = 0.5f;
    [SerializeField] float effectRadius = 10.0f;
    [SerializeField] float tickRate = 0.0f;
    float tickTimer = 0.0f;

    [Header("Ice Creation")]
    public bool canFreeze = false;
    [SerializeField] GameObject iceCreator;
    [SerializeField] float iceCreatorRelativeSize = 1.0f;
     IceCreator iceCreatorScript;

    [Header("Fuel Economy")]
    [SerializeField] float FuelGainRate = 100.0f;
    [SerializeField] float FuelLossRate = 100.0f;
    public float capacity = 1000.0f;
    [SerializeField] float toolStrength = 0.1f;
    [SerializeField] float toolStrengthChangeRate = 100.0f;
    [SerializeField] float maxToolStrength = 50.0f;
    [SerializeField] float minToolStrength = 6.0f;
    [HideInInspector] public float toolFuel = 0.0f;

    [Header("Camera")]
    [SerializeField] Camera playerCamera;

    [Header("Laser")]
    [SerializeField] GameObject laser;
    [SerializeField] Transform tool;
    [SerializeField] Transform laserStartPoint;
    [SerializeField] Material burnLaserMaterial;
    [SerializeField] Material freezeLaserMaterial;
    MeshRenderer laserRenderer;
    Vector3 laserScale = Vector3.zero;
    float laserLengthScalar;

    private void Start()
    {
        // get the ice creator script from the ice creator
        iceCreatorScript = iceCreator.GetComponent<IceCreator>();

        // set the ice creator to be at the furthest point the tool can hit
        iceCreator.transform.position = playerCamera.transform.position + playerCamera.transform.forward * maxRange;
        // point the tool towards the ice creator
        tool.LookAt(iceCreator.transform);

        // set the scale of the ice creator using iceCreatorRelativeSize, relative to the effect radius
        iceCreator.transform.localScale = new Vector3(effectRadius * 0.5f * iceCreatorRelativeSize, effectRadius * 0.5f * iceCreatorRelativeSize, effectRadius * 0.5f * iceCreatorRelativeSize);
        // set the ice creator to be inactive
        iceCreator.SetActive(false);

        // get the mesh renderer for the laser
        laserRenderer = laser.GetComponent<MeshRenderer>();
        // get the relative length the display beam needs to be relative to the actual spherecast distance. Half this value as as scale 1 cylinder is 2 units long
        laserLengthScalar = (new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, playerCamera.transform.position.z + maxRange) - laserStartPoint.position).magnitude / maxRange * 0.5f;
        // set the laser scale
        laserScale = new Vector3(laser.transform.localScale.x, maxRange * laserLengthScalar, laser.transform.localScale.z);
    }

    void Update()
    {
        // if the mouse wheel was scrolled, adjust the tool strength 
        toolStrength += Input.mouseScrollDelta.y * Time.deltaTime * toolStrengthChangeRate;
        // clamp the tool strength so it is within the min and max value
        toolStrength = Mathf.Clamp(toolStrength, minToolStrength, maxToolStrength);

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
                // reset the tick timer, saving excess time for next cycle
                tickTimer -= tickRate;

                // if the left click is down this frame or if the gun can freeze and right click was pressed this frame
                if (Input.GetMouseButton(0) || (canFreeze && Input.GetMouseButtonDown(1)))
                {
                    // set the ice creator to be inactive
                    iceCreator.SetActive(false);

                    // cast a spherecast forward from the center of the player camera viewport
                    Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
                    RaycastHit hit;
                    // if the spherecast hit a game object
                    if (Physics.SphereCast(ray, beamRadius, out hit, maxRange))
                    {
                        // position and scale the laser so it fires from the laser start point with a length based on the hit distance and a radius based on the beam radius
                        laser.transform.position = laserStartPoint.position + laserStartPoint.forward * hit.distance * laserLengthScalar;
                        laserScale.y = hit.distance * laserLengthScalar;
                        laser.transform.localScale = laserScale;

                        // if left click is down this frame
                        if (Input.GetMouseButton(0))
                        {
                            // if the hit gameobject has the tag "Ice", burn the ice at the point of the collision. If this succeeds and the tool is able to freeze ice
                            if (hit.transform.tag == "Ice" && hit.transform.GetComponent<EditableTerrain>().EditTerrain(false, hit.point, effectRadius, toolStrength) && canFreeze)
                            {
                                // increase the fuel by the fuel gain rate per second. Multiply the result by the tool strength
                                toolFuel += Time.deltaTime * FuelGainRate * toolStrength;
                                // if there is a capacity and the tool fuel is above that capacity
                                if (capacity > 0.0f && toolFuel > capacity)
                                {
                                    // set the fuel to be equal to the capacity
                                    toolFuel = capacity;
                                }
                            }
                        }
                        // if the tool is able to freeze ice, the mouse was right clicked this frame, the collision point was beyond the minimum creation distance and the tool has fuel
                        if (canFreeze && Input.GetMouseButtonDown(1) && hit.distance >= minimumFreezeDistance && toolFuel > 0.0f)
                        {
                            // freeze the ice at the point of the collision. If this succeeds
                            if (hit.transform.tag == "Ice" && hit.transform.GetComponent<EditableTerrain>().EditTerrain(true, hit.point, effectRadius, toolStrength))
                            {
                                // decrease the fuel by the fuel loss rate per second. Multiply the result by the tool strength
                                toolFuel -= Time.deltaTime * FuelLossRate * toolStrength;
                                // if there is less than 0 fuel
                                if (toolFuel < 0.0f)
                                {
                                    // set the fuel to be 0
                                    toolFuel = 0.0f;
                                }
                                
                                // set the ice creator to be active
                                iceCreator.SetActive(true);
                                // set the ice creator's position to be at the point of collision
                                iceCreator.transform.position = hit.point;
                            }
                        }
                    }
                    // if the spherecast did not hit a game object 
                    else
                    {
                        // position and scale the laser so it fires from the laser start point with a length based on the max range and a radius based on the beam radius
                        laser.transform.position = laserStartPoint.position + laserStartPoint.forward * maxRange * laserLengthScalar;
                        laserScale.y = maxRange * laserLengthScalar;
                        laser.transform.localScale = laserScale;
                    }
                }
                // else if the tool can freeze, right click is down this frame and the tool has fuel
                else if (canFreeze && Input.GetMouseButton(1) && toolFuel > 0.0f)
                {
                    // if the ice creator is colliding with ice and not the player
                    if (iceCreatorScript.ready)
                    {
                        // create ice at the ice creator
                        iceCreatorScript.iceTerrain.EditTerrain(true, iceCreator.transform.position, effectRadius, toolStrength);
                        // set the ice creator to not be ready so a collision check must occur again for ice to be validly generated
                        iceCreatorScript.ready = false;

                        // decrease the fuel by the fuel loss rate per second. Multiply the result by the tool strength
                        toolFuel -= Time.deltaTime * FuelLossRate * toolStrength;
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
        // if the mouse was not left clicked, or right clicked with fuel
        else
        {
            // set the ice creator to be inactive
            iceCreator.SetActive(false);
        }
    }
}
