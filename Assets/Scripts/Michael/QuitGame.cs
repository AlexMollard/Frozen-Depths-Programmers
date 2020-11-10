/*
    File name: QuitGame.cs
    Author: Michael Sweetman
    Summary: Quits the game if the player presses escape
    Creation Date: 10/11/2020
    Last Modified: 10/11/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        // if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // quit the application
            Application.Quit();
        }
    }
}
