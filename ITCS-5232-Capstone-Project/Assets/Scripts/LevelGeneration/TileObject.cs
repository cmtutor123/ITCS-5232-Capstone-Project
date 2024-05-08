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

    bool setOffset = false;

    Vector3 offset = new Vector3(0, 0, 0);

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
                if (!setOffset)
                {
                    SpriteRenderer sr = floors[0].GetComponentInChildren<SpriteRenderer>();
                    offset = new Vector3(sr.size.x / 2, -sr.size.y / 2);
                    setOffset = true;
                }
                foreach ((float, float) position in enemy.spawnPattern)
                {
                    GameObject matchEnemyObject = Instantiate(prefabMatchEnemy);
                    matchEnemyObject.transform.position = floors[0].transform.position + new Vector3(0, 0, -2) + new Vector3(position.Item1, position.Item2, 0) + offset;
                    MatchEnemy matchEnemy = matchEnemyObject.GetComponent<MatchEnemy>();
                    matchEnemy.SetEnemy(enemy);
                    GameManager.instance.RegisterEnemy(matchEnemy);
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
        Destroy(gameObject);
    }
}
