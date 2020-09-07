/*
    File name: Key.cs
    Author:    Luke Lazzaro
    Summary: Adds key functionality to interactable objects
    Creation Date: 31/08/2020
    Last Modified: 7/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private string id = "";

    public void Collect()
    {
        if (!string.IsNullOrEmpty(id))
        {
            Debug.Log("Collected!");
            KeyManager.Instance.keys.Add(id);
            Destroy(gameObject);
        }
        else
            Debug.LogError("Please enter an ID for your key!");
    }
}
