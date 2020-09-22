/*
    File name: IceCreator.cs
    Author: Michael Sweetman
    Summary: determines whether ice creation is valid
    Creation Date: 22/09/2020
    Last Modified: 22/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreator : MonoBehaviour
{
    [SerializeField] GameObject player;
    [HideInInspector] public bool ready;
    [HideInInspector] public EditableTerrain iceTerrain;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ice")
        {
            iceTerrain = other.GetComponent<EditableTerrain>();
            ready = true;
        }

        if (other.gameObject == player)
        {
            ready = false;
        }
    }
}
