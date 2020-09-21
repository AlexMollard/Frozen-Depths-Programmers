/*
    File name: Keyhole.cs
    Author:    Luke Lazzaro
    Summary: Does something if the player has a required key
    Creation Date: 31/08/2020
    Last Modified: 21/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum OpenBehaviour
{
    RisingDoor
}

public class Keyhole : MonoBehaviour
{
    [SerializeField] private string id = "";
    [SerializeField] private GameObject objectToOpen;
    [SerializeField] private OpenBehaviour openBehaviour = OpenBehaviour.RisingDoor;
    [SerializeField] private GameObject placeForKeyMesh;

    [Header("Rising Door")]
    [SerializeField] private Vector3 targetPos = new Vector3();
    [SerializeField] private float doorRisingSpeed = 10;

    private Vector3 originalPos;
    private bool isRising = false;

    private void Start()
    {
        originalPos = objectToOpen.transform.position;

        if (string.IsNullOrEmpty(id))
            Debug.LogError("One of your keyholes doesn't have an ID!");
    }

    public void Open()
    {
        if (!KeyManager.Instance.keys.Contains(id))
        {
            Debug.Log("No key matches this keyhole.");
            return;
        }

        switch (openBehaviour)
        {
            case OpenBehaviour.RisingDoor:
                Debug.Log("Opening door...");
                isRising = true;

                AudioSource source = objectToOpen.GetComponent<AudioSource>();
                if (source.clip != null)
                    source.Play();

                break;
            default:
                break;
        }

        PlaceKeyOnKeyhole();
    }

    private void PlaceKeyOnKeyhole()
    {
        foreach (KeyLookup item in KeyManager.Instance.keyLookup)
        {
            if (id == item.keyId)
            {
                placeForKeyMesh.SetActive(true);
                placeForKeyMesh.GetComponent<MeshFilter>().mesh = item.keyObject.GetComponent<MeshFilter>().mesh;
                placeForKeyMesh.GetComponent<MeshRenderer>().material = item.keyObject.GetComponent<MeshRenderer>().material;
            }
        }
    }

    private void Update()
    {
        if (isRising)
        {
            objectToOpen.transform.position = Vector3.MoveTowards(objectToOpen.transform.position, originalPos + targetPos, doorRisingSpeed * Time.deltaTime);

            if (objectToOpen.transform.position == originalPos + targetPos)
            {
                Debug.Log("Done!");
                isRising = false;
            }
        }
    }
}
