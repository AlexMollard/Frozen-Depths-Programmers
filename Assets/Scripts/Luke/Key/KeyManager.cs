/*
    File name: KeyManager.cs
    Author:    Luke Lazzaro
    Summary: Manages keys can be picked up
    Creation Date: 31/08/2020
    Last Modified: 7/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }
    public List<string> keys = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
}
