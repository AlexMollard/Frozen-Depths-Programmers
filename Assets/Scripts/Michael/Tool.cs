/*
    File name: Tool.cs
    Author: Michael Sweetman
    Summary: Determines a point on the ice mesh the player wants burnt/frozen. Manages a fuel to limit the use of ice creation.
    Creation Date: 21/07/2020
    Last Modified: 30/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Tool : MonoBehaviour
{
    public class Laser
    {
        // laser constructor
        public Laser(GameObject laserGameObject, Transform laserStartPoint, Vector3 cameraPosition, float maxRange)
        {
            // store the laser game object and laser start point
            gameObject = laserGameObject;
            startPoint = laserStartPoint;

            // get the relative length the display beam needs to be relative to the actual spherecast distance. Half this value as as scale 1 cylinder is 2 units long
            lengthScalar = (new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z + maxRange) - startPoint.position).magnitude / maxRange * 0.5f;
            // get the starting laser scale
            scale = laserGameObject.transform.localScale;
            // set the length and position of the laser
            SetLength(maxRange);
        }

        // positions and scales the laser so it fires from the laser start point with a length based on the argument length
        public void SetLength(float length)
        {
            // position the laser so when it is scaled, its end point are at the start point and the point the start point is facing 'length' units away
            gameObject.transform.position = startPoint.position + startPoint.forward * length * lengthScalar;
            // determine the scale of the laser using the length scalar and the argument length
            scale.y = length * lengthScalar * 0.5f;
            // apply the scale to the laser game object
            gameObject.transform.localScale = scale;
        }

        public GameObject gameObject;
        public Transform startPoint;
        public float lengthScalar;
        public Vector3 scale;
    }

    [Header("Tool Use")]
    [SerializeField] float minimumFreezeDistance = 2.0f;
    [SerializeField] float maxRange = 10.0f;
    [SerializeField] float beamRadius = 0.5f;
    [SerializeField] float effectRadius = 10.0f;
    [SerializeField] float tickRate = 0.0f;
    float tickTimer = 0.0f;
    Ray ray;
    RaycastHit hit;

    [Header("Ice Creation")]
    public bool canFreeze = false;
    [SerializeField] GameObject iceCreator;
    [SerializeField] float iceCreatorRelativeSize = 1.0f;
    [SerializeField] float iceCreatorMoveSpeed = 0.1f;
    float iceCreatorMinimumMovement = 0.01f;
    IceCreator iceCreatorScript;
    float distanceFromPlayer = 0.0f;

    [Header("Fuel Economy")]
    [SerializeField] float FuelGainRate = 100.0f;
    [SerializeField] float FuelLossRate = 100.0f;
    public float capacity = 1000.0f;
    [SerializeField] float freezeStrength = 0.1f;
    [SerializeField] float meltStrength = 0.1f;
    [HideInInspector] public float toolFuel = 0.0f;

    [Header("Camera")]
    [SerializeField] Camera playerCamera;

    [Header("Lasers")]
    [SerializeField] Transform tool;
    [SerializeField] GameObject meltLaserObject;
    [SerializeField] Transform meltLaserStartPoint;
    [SerializeField] GameObject freezeLaserObject;
    [SerializeField] Transform freezeLaserStartPoint;
    Laser meltLaser;
    Laser freezeLaser;
    Laser currentLaser;

    [Header("Crosshair")]
    [SerializeField] Crosshair crosshair;

    [Header("Particles")]
    [SerializeField] Transform steamParticleEmitter;
    [SerializeField] Transform sleetParticleEmitter;
    [SerializeField] Transform mistParticleEmitter;
    [SerializeField] ParticleSystem barrelLightParticleEmitter;
    [SerializeField] int steamParticleCount = 4;
    [SerializeField] int sleetParticleCount = 4;
    [SerializeField] int mistParticleCount = 4;
    [SerializeField] int barrelLightParticleCount = 4;
    ParticleSystem steamParticleSystem;
    ParticleSystem sleetParticleSystem;
    ParticleSystem mistParticleSystem;

    [Header("Sound")]
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip meltLoop;
    [SerializeField] AudioClip meltEnd;
    [SerializeField] AudioClip freezeLoop;
    [SerializeField] AudioClip freezeEnd;

    // plays a desired sound
    void PlaySound(AudioClip sound, bool loop)
    {
        // set whether the audio player loops based on the argument value
        audioPlayer.loop = loop;
        // set the audio player to play the desired sound
        audioPlayer.clip = sound;
        audioPlayer.Play();
    }

    private void Start()
    {
        // get the ice creator script from the ice creator
        iceCreatorScript = iceCreator.GetComponent<IceCreator>();

        // set the ice creator to be at the furthest point the tool can hit
        iceCreator.transform.position = playerCamera.transform.position + playerCamera.transform.forward * maxRange;
        // point the tool towards the ice creator
        tool.LookAt(iceCreator.transform);

        // point the laser start points towards the ice creator
        meltLaserStartPoint.LookAt(iceCreator.transform);
        freezeLaserStartPoint.LookAt(iceCreator.transform);

        // set the scale of the ice creator using iceCreatorRelativeSize, relative to the effect radius
        iceCreator.transform.localScale = new Vector3(effectRadius * 0.5f * iceCreatorRelativeSize, effectRadius * 0.5f * iceCreatorRelativeSize, effectRadius * 0.5f * iceCreatorRelativeSize);
        // set the ice creator to be inactive
        iceCreator.SetActive(false);

        meltLaser = new Laser(meltLaserObject, meltLaserStartPoint, playerCamera.transform.position, maxRange);
        freezeLaser = new Laser(freezeLaserObject, freezeLaserStartPoint, playerCamera.transform.position, maxRange);

        // set the crosshair to use the out of range image
        crosshair.SetImage(false);

        // get the particle system component from the particle emitters
        steamParticleSystem = steamParticleEmitter.GetComponent<ParticleSystem>();
        sleetParticleSystem = sleetParticleEmitter.GetComponent<ParticleSystem>();
        mistParticleSystem = mistParticleEmitter.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // set the lasers to be inactive
        meltLaser.gameObject.SetActive(false);
        freezeLaser.gameObject.SetActive(false);

        // increase the timer by the amount of time passed since last frame
        tickTimer += Time.deltaTime;

        // if the ice creator is inactive
        if (!iceCreator.activeSelf)
        {
            // cast a spherecast forward from the center of the player camera viewport
            ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
            // if the spherecast hit a gameobject with the tag ice set the crosshair to use the within range image. Use the out of range image otherwise.
            crosshair.SetImage(Physics.SphereCast(ray, beamRadius, out hit, maxRange) && hit.transform.tag == "Ice");
        }

        // if the mouse is left clicked, or right clicked with fuel
        if ((Input.GetMouseButton(0)|| (Input.GetMouseButton(1) && toolFuel > 0.0f)))
        {
            // emit the light from the tool barrel
            barrelLightParticleEmitter.Emit(barrelLightParticleCount);

            // if the tool can freeze, or if the mouse was left clicked
            if (canFreeze || Input.GetMouseButton(0))
            {
                // activate the laser corresponding to the player's chosen action
                currentLaser = (Input.GetMouseButton(0)) ? meltLaser : freezeLaser;
                currentLaser.gameObject.SetActive(true);
            }

            // if enough time has passed since the last terrain edit
            if (tickTimer > tickRate)
            {
                // reset the tick timer, saving excess time for next cycle
                tickTimer -= tickRate;

                // if the left click is down this frame or if the gun can freeze, right click is down and the ice creator is inactive
                if (Input.GetMouseButton(0) || (canFreeze && Input.GetMouseButton(1) && !iceCreator.activeSelf))
                {
                    // set the ice creator to be inactive
                    iceCreator.SetActive(false);

                    // if the spherecast hit a game object
                    if (Physics.SphereCast(ray, beamRadius, out hit, maxRange))
                    {
                        // set the current laser to be as long as the hit distance
                        currentLaser.SetLength(hit.distance);

                        // if left click is down this frame
                        if (Input.GetMouseButton(0))
                        {
                            // if the hit gameobject has the tag "Ice", burn the ice at the point of the collision. If this succeeds
                            if (hit.transform.tag == "Ice" && hit.transform.GetComponent<EditableTerrain>().EditTerrain(false, hit.point, effectRadius, freezeStrength, meltStrength))
                            {
                                // emit steam particles at the collision point
                                steamParticleEmitter.position = hit.point;
                                steamParticleSystem.Emit(steamParticleCount);
                                // emit sleet particles at the collision point
                                sleetParticleEmitter.position = hit.point;
                                sleetParticleSystem.Emit(sleetParticleCount);

                                // if the tool can freeze
                                if (canFreeze)
                                {
                                    // increase the fuel by the fuel gain rate per second. Multiply the result by the melt strength
                                    toolFuel += Time.deltaTime * FuelGainRate * meltStrength;
                                    // if there is a capacity and the tool fuel is above that capacity
                                    if (capacity > 0.0f && toolFuel > capacity)
                                    {
                                        // set the fuel to be equal to the capacity
                                        toolFuel = capacity;
                                    }
                                }
                            }

                            // if the mouse was left clicked this frame
                            if (Input.GetMouseButtonDown(0))
                            {
                                // play the meltloop clip 
                                PlaySound(meltLoop, true);
                            }
                        }
                        // if the tool is able to freeze ice, the mouse was right clicked this frame, the collision point was beyond the minimum creation distance and the tool has fuel
                        if (canFreeze && Input.GetMouseButtonDown(1) && hit.distance >= minimumFreezeDistance && toolFuel > 0.0f)
                        {
                            // freeze the ice at the point of the collision. If this succeeds
                            if (hit.transform.tag == "Ice" && hit.transform.GetComponent<EditableTerrain>().EditTerrain(true, hit.point, effectRadius, freezeStrength, meltStrength))
                            {
                                // decrease the fuel by the fuel loss rate per second. Multiply the result by the freeze strength
                                toolFuel -= Time.deltaTime * FuelLossRate * freezeStrength;
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
                                // store the ice Creator's distance from the player
                                distanceFromPlayer = hit.distance;

                                // emit mist particles at the collision point
                                mistParticleEmitter.position = hit.point;
                                mistParticleSystem.Emit(mistParticleCount);

                                // play the freeze loop clip
                                PlaySound(freezeLoop, true);
                            }
                        }
                    }
                    // if the spherecast did not hit a game object 
                    else
                    {
                        // set the current laser to be as long as the max range
                        currentLaser.SetLength(maxRange);
                    }
                }
                // else if the tool can freeze, right click is down this frame and the tool has fuel
                else if (canFreeze && Input.GetMouseButton(1) && toolFuel > 0.0f)
                {
                    //if the ice creator is not to close
                    if (distanceFromPlayer > minimumFreezeDistance)
                    {
                        // get the mouse movement this frame
                        float mouseX = Input.GetAxis("Mouse X");
                        float mouseY = Input.GetAxis("Mouse Y");
                        // if the mouse did not move too much
                        if (mouseX > -iceCreatorMinimumMovement && mouseX < iceCreatorMinimumMovement && mouseY > -iceCreatorMinimumMovement && mouseY < iceCreatorMinimumMovement)
                        {
                            // move the ice creator closer to the player camera
                            iceCreator.transform.position -= playerCamera.transform.forward * iceCreatorMoveSpeed * Time.deltaTime;
                            // determine the new distance from the player
                            distanceFromPlayer -= iceCreatorMoveSpeed * Time.deltaTime;
                            // adjust the length of the laser
                            currentLaser.SetLength(distanceFromPlayer);
                        }
                    }

                    // if the ice creator is colliding with ice and not the player
                    if (iceCreatorScript.ready)
                    {
                        // set the crosshair to use the within range image
                        crosshair.SetImage(true);
                        // create ice at the ice creator
                        iceCreatorScript.iceTerrain.EditTerrain(true, iceCreatorScript.collisionPoint, effectRadius, freezeStrength, meltStrength);
                        // set the ice creator to not be ready so a collision check must occur again for ice to be validly generated
                        iceCreatorScript.ready = false;

                        // emit mist particles at the ice creator's collision point
                        mistParticleEmitter.position = iceCreatorScript.collisionPoint;
                        mistParticleSystem.Emit(mistParticleCount);

                        // decrease the fuel by the fuel loss rate per second. Multiply the result by the freeze strength
                        toolFuel -= Time.deltaTime * FuelLossRate * freezeStrength;
                        // if there is less than 0 fuel
                        if (toolFuel < 0.0f)
                        {
                            // set the fuel to be 0
                            toolFuel = 0.0f;
                        }
                    }
                    // else if the ice creator is not ready
                    else
                    {
                        crosshair.SetImage(false);
                    }
                }
            }
        }
        // if the mouse was not left clicked, or right clicked with fuel
        else
        {
            // if the audio player is currently playing the meltLoop clip
            if (audioPlayer.isPlaying && audioPlayer.clip == meltLoop)
            {
                // play the meltEnd clip
                PlaySound(meltEnd, false);
            }
            // if the audio player is currently playing the freezeLoop clip
            else if (audioPlayer.isPlaying && audioPlayer.clip == freezeLoop)
            {
                // play the freezeEnd clip
                PlaySound(freezeEnd, false);
            }

            // set the ice creator to be inactive
            iceCreator.SetActive(false);
        }
    }
}
