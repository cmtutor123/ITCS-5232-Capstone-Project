using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int enemyLevel;
}
