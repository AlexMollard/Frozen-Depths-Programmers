/*
    File name: DeathTrigger.cs
    Author:    Luke Lazzaro
    Summary: Causes death when attached game object is touched
    Creation Date: 8/09/2020
    Last Modified: 8/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        other.GetComponent<PlayerMovement>().willDie = true;
    }
}
