using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public GameObject tileObjectPrefab, floorPrefab, wallPrefab, doorPrefab;
    public float tileSize = 30;

    public TileObject GenerateTile(TileData tileData)
    {
        TileObject tileObject = Instantiate(tileObjectPrefab).GetComponent<TileObject>();
        float x = tileData.tileOffset.Item1;
        float y = tileData.tileOffset.Item2;
        x *= tileSize;
        y *= tileSize;
        tileObject.transform.position = new Vector3(x, y, 0);
        GameObject floor = Instantiate(floorPrefab, tileObject.transform, true);
        floor.transform.localScale = new Vector3(tileSize, tileSize, 1);
        floor.transform.localPosition = new Vector3(0, 0, 0);
        tileObject.floors.Add(floor);
        if (tileData.wallLeft)
        {
            GameObject wall = Instantiate(wallPrefab, tileObject.transform, true);
            wall.transform.localScale = new Vector3(1, tileSize, 1);
            wall.transform.localPosition = new Vector3(-tileSize / 2, 0, 0);
            tileObject.walls.Add(wall);
        }
        if (tileData.wallRight)
        {
            GameObject wall = Instantiate(wallPrefab, tileObject.transform, true);
            wall.transform.localScale = new Vector3(1, tileSize, 1);
            wall.transform.localPosition = new Vector3(tileSize / 2, 0, 0);
            tileObject.walls.Add(wall);
        }
        if (tileData.wallDown)
        {
            if (!tileData.hasEntrance)
            {
                GameObject wall = Instantiate(wallPrefab, tileObject.transform, true);
                wall.transform.localScale = new Vector3(tileSize, 1, 1);
                wall.transform.localPosition = new Vector3(0, -tileSize / 2, 0);
                tileObject.walls.Add(wall);
            }
            else
            {
                GameObject wall = Instantiate(wallPrefab, tileObject.transform, true);
                wall.transform.localScale = new Vector3(tileSize / 3, 1, 1);
                wall.transform.localPosition = new Vector3(-tileSize / 3, -tileSize / 2, 0);
                tileObject.walls.Add(wall);
                wall = Instantiate(wallPrefab, tileObject.transform, true);
                wall.transform.localScale = new Vector3(tileSize / 3, 1, 1);
                wall.transform.localPosition = new Vector3(tileSize / 3, -tileSize / 2, 0);
                tileObject.walls.Add(wall);
            }
        }
        if (tileData.wallUp)
        {
            if (!tileData.hasExit)
            {
                GameObject wall = Instantiate(wallPrefab, tileObject.transform, true);
                wall.transform.localScale = new Vector3(tileSize, 1, 1);
                wall.transform.localPosition = new Vector3(0, tileSize / 2, 0);
                tileObject.walls.Add(wall);
            }
            else
            {
                GameObject wall = Instantiate(wallPrefab, tileObject.transform, true);
                wall.transform.localScale = new Vector3(tileSize / 3, 1, 1);
                wall.transform.localPosition = new Vector3(-tileSize / 3, tileSize / 2, 0);
                tileObject.walls.Add(wall);
                wall = Instantiate(wallPrefab, tileObject.transform, true);
                wall.transform.localScale = new Vector3(tileSize / 3, 1, 1);
                wall.transform.localPosition = new Vector3(tileSize / 3, tileSize / 2, 0);
                tileObject.walls.Add(wall);
                GameObject door = Instantiate(doorPrefab, tileObject.transform, true);
                door.transform.localScale = new Vector3(tileSize / 3, 1, 1);
                door.transform.localPosition = new Vector3(0, tileSize / 2, 0);
                tileObject.doors.Add(door);
                door.GetComponent<MatchDoor>().index = tileData.index;
            }
        }
        return tileObject;
    }
}
