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
    [SerializeField] GameObject antidoteMenu;

    void Update()
    {
        // set the antidote menu to be active if the tab key is down, inactive otherwise
        antidoteMenu.SetActive(Input.GetKey(KeyCode.Tab));
    }
}