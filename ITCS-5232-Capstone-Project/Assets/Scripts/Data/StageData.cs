using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage Data", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    public string stageName;

    public int roomCount = 10;
    public int[] powerLevels;

    public EnemyData[] normalEnemies, strongEnemies, minibossEnemies, bossEnemies;

    public TileGenerator tileGenerator;
}
