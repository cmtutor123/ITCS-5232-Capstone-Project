using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int enemyLevel;
    public float flatHealth, scaledHealth;
    public float flatDamage, scaledDamage;
    public float moveSpeed;
    public Sprite sprite;
    public float sizeX, sizeY;
    public List<(float, float)> spawnPattern => GetSpawnPattern();
    public List<float> spawnX, spawnY;
    public List<(float, float)> spawnXY;

    public bool isBoss = false;

    public List<(float, float)> GetSpawnPattern()
    {
        if (spawnXY == null)
        {
            spawnXY = new List<(float, float)>();
            for (int i = 0; i < spawnX.Count; i++)
            {
                spawnXY.Add((spawnX[i], spawnY[i]));
            }
        }
        return spawnXY;
    }
}
