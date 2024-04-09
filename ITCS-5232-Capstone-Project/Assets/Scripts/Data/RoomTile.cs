using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Tile", menuName = "RoomTile")]
public class RoomTile : ScriptableObject
{
    List<string> formatId;
    public bool wallLeft, wallRight, wallUp, wallDown, hasEntrance, hasExit;
    public TileGenerator tileGenerator;

    public List<string> GetIds()
    {
        return formatId;
    }

    public bool HasId(string id)
    {
        return formatId.Contains(id);
    }
}
