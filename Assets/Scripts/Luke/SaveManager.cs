﻿using Microsoft.SqlServer.Server;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    public static void SaveGame(GameObject player)
    {
        SaveData data = new SaveData();
        data.playerPosX = player.transform.position.x;
        data.playerPosY = player.transform.position.y;
        data.playerPosZ = player.transform.position.z;
        data.toolFuel = player.GetComponent<Tool>().toolFuel;
        // save terrain mesh here

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedata");
        bf.Serialize(file, data);
        file.Close();
    }

    // Call this method in LateUpdate, otherwise the position will get overwritten by other movement logic
    public static void LoadGame(GameObject player)
    {
        if (File.Exists(Application.persistentDataPath + "/savedata"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedata", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            Debug.Log(player.transform.position);
            Debug.Log("Saved pos: " + data.playerPosX + ", " + data.playerPosY + ", " + data.playerPosZ);
            player.transform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
            Debug.Log(player.transform.position);
            player.GetComponent<Tool>().toolFuel = data.toolFuel;
        }
    }
}
