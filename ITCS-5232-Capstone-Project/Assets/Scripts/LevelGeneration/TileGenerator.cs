using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public GameObject tileObjectPrefab, floorPrefab, wallPrefab, doorPrefab;
    public static float tileSize = 10;
    TileObject tileObject;

    public GameObject SpawnStageObject(GameObject prefab, Vector3 size, Vector3 center)
    {
        GameObject stageObject = Instantiate(prefab, tileObject.transform, true);
        stageObject.transform.localPosition = center - ((new Vector3(size.x, -size.y, size.z)) / 2);
        BoxCollider2D collider = stageObject.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = size;
            collider.offset = new Vector2(size.x / 2, -size.y / 2);
        }
        SpriteRenderer spriteRenderer = stageObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.size = size;
        }
        return stageObject;
    }

    public TileObject GenerateTile(TileData tileData)
    {
        tileObject = Instantiate(tileObjectPrefab).GetComponent<TileObject>();
        float x = tileData.tileOffset.Item1;
        float y = tileData.tileOffset.Item2;
        x *= tileSize;
        y *= tileSize;
        tileObject.transform.position = new Vector3(x, y, 0);

        tileObject.floors.Add(SpawnStageObject(floorPrefab, new Vector3(tileSize, tileSize, 1), new Vector3(0, 0, 0)));

        if (tileData.wallLeft)
        {
            tileObject.walls.Add(SpawnStageObject(wallPrefab, new Vector3(1, tileSize + 1, 1), new Vector3(-tileSize / 2, 0, 0)));
        }
        if (tileData.wallRight)
        {
            tileObject.walls.Add(SpawnStageObject(wallPrefab, new Vector3(1, tileSize + 1, 1), new Vector3(tileSize / 2, 0, 0)));
        }
        if (tileData.wallDown)
        {
            if (!tileData.hasEntrance)
            {
                tileObject.walls.Add(SpawnStageObject(wallPrefab, new Vector3(tileSize + 1, 1, 1), new Vector3(0, -tileSize / 2, 0)));
            }
            else
            {
                tileObject.walls.Add(SpawnStageObject(wallPrefab, new Vector3(tileSize / 3, 1, 1), new Vector3(-tileSize / 3, -tileSize / 2, 0)));

                tileObject.walls.Add(SpawnStageObject(wallPrefab, new Vector3(tileSize / 3, 1, 1), new Vector3(tileSize / 3, -tileSize / 2, 0)));
            }
        }
        if (tileData.wallUp)
        {
            if (!tileData.hasExit)
            {
                tileObject.walls.Add(SpawnStageObject(wallPrefab, new Vector3(tileSize + 1, 1, 1), new Vector3(0, tileSize / 2, 0)));
            }
            else
            {
                tileObject.walls.Add(SpawnStageObject(wallPrefab, new Vector3(tileSize / 3, 1, 1), new Vector3(-tileSize / 3, tileSize / 2, 0)));

                tileObject.walls.Add(SpawnStageObject(wallPrefab, new Vector3(tileSize / 3, 1, 1), new Vector3(tileSize / 3, tileSize / 2, 0)));

                Vector3 size = new Vector3(tileSize / 3, 1, 1);
                Vector3 center = new Vector3(0, tileSize / 2, 0);
                GameObject door = SpawnStageObject(doorPrefab, size, center);
                door.GetComponent<MatchDoor>().index = tileData.index;
                door.transform.position += new Vector3(size.x / 2, -size.y / 2, 0);
                tileObject.doors.Add(door);
            }
        }
        return tileObject;
    }
}
