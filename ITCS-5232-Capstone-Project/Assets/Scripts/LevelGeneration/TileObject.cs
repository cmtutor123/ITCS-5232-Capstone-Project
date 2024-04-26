using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public float spawnDelay => GameManager.instance.spawnDelay;
    public GameObject prefabMatchEnemy => GameManager.instance.prefabMatchEnemy;

    public List<GameObject> floors = new List<GameObject>();
    public List<GameObject> walls = new List<GameObject>();
    public List<GameObject> doors = new List<GameObject>();

    public List<EnemyData> enemyQueue = new List<EnemyData>();

    public float spawnTimer;

    void FixedUpdate()
    {
        if (enemyQueue.Count > 0)
        {
            spawnTimer += Time.fixedDeltaTime;
            if (spawnTimer >= spawnDelay)
            {
                spawnTimer = 0;
                EnemyData enemy = enemyQueue[0];
                enemyQueue.RemoveAt(0);
                foreach ((float, float) position in enemy.spawnPattern)
                {
                    GameObject matchEnemy = Instantiate(prefabMatchEnemy);
                    matchEnemy.transform.position = floors[0].transform.position + new Vector3(0, 0, -2) + new Vector3(position.Item1, position.Item2, 0);
                    matchEnemy.GetComponent<MatchEnemy>().SetEnemy(enemy);
                }
            }
        }
    }

    public void AddEnemyToQueue(EnemyData enemy)
    {
        enemyQueue.Add(enemy);
    }

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
