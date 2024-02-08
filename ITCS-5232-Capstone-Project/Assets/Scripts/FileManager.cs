using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class FileManager
{
    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText("playerData.json", json);
    }

    public static PlayerData LoadPlayerData()
    {
        if (!File.Exists("playerData.json"))
        {
            SavePlayerData(new PlayerData());
        }
        string json = System.IO.File.ReadAllText("playerData.json");
        PlayerData loadedPlayerData = JsonUtility.FromJson<PlayerData>(json);
        return loadedPlayerData;
    }
}
