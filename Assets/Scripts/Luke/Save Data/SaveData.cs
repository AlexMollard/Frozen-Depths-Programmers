/*
    File name: SaveData.cs
    Author:    Luke Lazzaro
    Summary: Used to store data that needs to be saved and loaded
    Creation Date: 3/08/2020
    Last Modified: 7/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public float toolFuel;

    public int currentCheckpoint;
    public List<string> keys;

    // TODO: multidimensional array of floats for the terrain mesh
}
