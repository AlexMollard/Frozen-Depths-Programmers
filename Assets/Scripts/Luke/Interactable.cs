/*
    File name: Interactable.cs
    Author:    Luke Lazzaro
    Summary: Object to be put in the artifact viewer
    Creation Date: ??/??/2020
    Last Modified: 10/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string id = "";
    public ArtifactDisplay artifactDisplay;
    [TextArea] public string description = "";
    public bool playerCanMove = false;
}
