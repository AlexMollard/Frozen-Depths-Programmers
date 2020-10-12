/*
    File name: AntidoteMenuHider.cs
    Author: Michael Sweetman
    Summary: Shows the antidote menu when the player holds down a key
    Creation Date: 12/10/2020
    Last Modified: 12/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntidoteMenuHider : MonoBehaviour
{
    public GameObject antidoteMenu;

    void Update()
    {
        antidoteMenu.SetActive(Input.GetKey(KeyCode.Tab));
    }
}