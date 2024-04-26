using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public int difficultyLevel => GameManager.instance.GetDifficultyLevel();

    public TileGenerator tileGenerator;
    public List<TileObject> tileObjects = new List<TileObject>();

    public void GenerateTile(TileData tileData)
    {
        tileObjects.Add(tileGenerator.GenerateTile(tileData));
    }

    public void DestroyRoom()
    {
        foreach (TileObject tileObject in tileObjects)
        {
            tileObject.DestroyTile();
        }
        Destroy(gameObject);
    }

    public void SpawnWave(List<EnemyData> enemies)
    {
        foreach (EnemyData enemy in enemies)
        {
            Debug.Log("Spawning Power " + enemy.enemyLevel);
        }
    }
}
