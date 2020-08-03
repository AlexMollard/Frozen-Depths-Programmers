using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameTest : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveManager.SaveGame(gameObject);
            Debug.Log("Saved!");
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            SaveManager.LoadGame(gameObject);
            Debug.Log("Loaded!");
        }
    }
}
