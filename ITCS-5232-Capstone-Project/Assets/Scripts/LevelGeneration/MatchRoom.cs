using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRoom
{
    public RoomData roomData;
    public TileGenerator tileGenerator;
    public RoomObject roomObject;

    public MatchRoom(RoomData roomData, TileGenerator tileGenerator)
    {
        this.roomData = roomData;
        this.tileGenerator = tileGenerator;
    }

    public void GenerateRoom()
    {
        roomObject = GameObject.Instantiate(GameManager.instance.roomObjectPrefab).GetComponent<RoomObject>();
        roomObject.transform.position = new Vector3(0, 0, 0);
        roomObject.tileGenerator = tileGenerator;
        foreach (TileData tileData in roomData.tileData)
        {
            roomObject.GenerateTile(tileData);
        }
    }

    public void SpawnWave(int index)
    {
        roomObject.SpawnWave(roomData.roomEnemies);
    }

    public void DestroyRoom()
    {
        roomObject.DestroyRoom();
    }
}
