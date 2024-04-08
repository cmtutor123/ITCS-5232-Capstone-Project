using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public bool hasEntrance, hasExit, wallLeft, wallRight, wallUp, wallDown;
    public (int, int) tileOffset;

    public TileData((int, int) offset, bool left, bool right, bool up, bool down)
    {
        tileOffset = offset;
        wallLeft = left;
        wallRight = right;
        wallUp = up;
        wallDown = down;
    }
}
