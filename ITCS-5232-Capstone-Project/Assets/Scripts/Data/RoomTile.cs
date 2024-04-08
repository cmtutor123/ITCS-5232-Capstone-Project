using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Tile", menuName = "RoomTile")]
public class RoomTile : ScriptableObject
{
    string formatId = null;
    public bool wallLeft, wallRight, wallUp, wallDown, hasEntrance, hasExit;
    public Func<(int, int), GameObject> GenerationFunction;

    public string GetId()
    {
        if (formatId == null)
        {
            formatId = GameManager.GetRoomFormatId(wallLeft, wallRight, wallUp, wallDown, hasEntrance, hasExit);
        }
        return formatId;
    }
}
