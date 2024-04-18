using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public List<GameObject> floors = new List<GameObject>();
    public List<GameObject> walls = new List<GameObject>();
    public List<GameObject> doors = new List<GameObject>();

    public void DestroyTile()
    {
        foreach (GameObject floor in floors)
        {
            Destroy(floor);
        }
        foreach (GameObject wall in walls)
        {
            Destroy(wall);
        }
        foreach (GameObject door in doors)
        {
            Destroy(door);
        }
    }
}
